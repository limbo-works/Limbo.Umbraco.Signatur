using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Constants;
using Limbo.Umbraco.Signatur.Extensions;
using Limbo.Umbraco.Signatur.Settings;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Exceptions;
using Skybrud.Essentials.Time;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Limbo.Umbraco.Signatur.Services;

public class SignaturJobsService {

    private readonly IContentService _contentService;
    private readonly ISignaturFeedParser _signaturFeedParser;
    private readonly SignaturSettings _settings;

    public SignaturJobsService(IContentService contentService, ISignaturFeedParser signaturFeedParser, IOptions<SignaturSettings> settings) {
        _contentService = contentService;
        _signaturFeedParser = signaturFeedParser;
        _settings = settings.Value;
    }

    public void Import(SignaturFeedSettings feed) {

        // Validate the feed
        if (string.IsNullOrWhiteSpace(feed.Url)) throw new PropertyNotSetException(nameof(feed.Url));
        if (feed.ParentContentKey == Guid.Empty) throw new PropertyNotSetException(nameof(feed.ParentContentKey));
        if (string.IsNullOrWhiteSpace(feed.ContentTypeAlias)) throw new PropertyNotSetException(nameof(feed.ContentTypeAlias));

        // Get the parent content node
        IContent? parent = _contentService.GetById(feed.ParentContentKey);
        if (parent is null) throw new BjernerSaysNoException($"Parent with key '{feed.ParentContentKey}' not found.");

        #region Get existing content/jobs

        Dictionary<int, IContent> existing = new();

        foreach (IContent content in _contentService.GetPagedChildren(parent.Id, 0, int.MaxValue, out long _)) {

            if (content.ContentType.Alias != feed.ContentTypeAlias) continue;

            int jobId = content.GetValue<int>("signaturJobId");
            if (jobId == 0) continue;

            existing[jobId] = content;

        }

        #endregion

        #region Get the jobs from the RSS feed

        ISignaturFeed rssFeed = _signaturFeedParser.Load(feed.Url);

        #endregion

        #region Add or update the jobs in Umbraco

        foreach (ISignaturItem item in rssFeed.Items) {
            AddOrUpdate(item, parent, feed, existing);
        }

        #endregion

        #region Delete jobs no longer in the feed

        // TODO: Delete jobs no longer in the feed

        #endregion

    }

    protected virtual void AddOrUpdate(ISignaturItem item, IContent parent, SignaturFeedSettings feed, Dictionary<int, IContent> existing) {

        // If the content doesn't already exist, we create in
        if (!existing.TryGetValue(item.WebAdId, out IContent? content)) {
            content = _contentService.Create(item.Title, parent, feed.ContentTypeAlias, _settings.ImportUserId);
        }

        // Update the Umbraco properties based on the job item
        bool modified = UpdateProperties(item, content, content.Id == 0);

        // Save and published the content item if we detecthed any changes
        if (modified) _contentService.SaveAndPublish(content, userId: _settings.ImportUserId);

        existing.Remove(item.WebAdId);

    }

    protected virtual bool UpdateProperties(ISignaturItem item, IContent content, bool isNew) {

        if (!content.TryGetProperty(SignaturProperties.JobId, out IProperty? idProperty)) {
            throw new BjernerSaysNoException($"Required property '{SignaturProperties.JobId}' not found for content type '{content.ContentType.Alias}'.");
        }

        if (!content.TryGetProperty(SignaturProperties.JobData, out IProperty? dataProperty)) {
            throw new BjernerSaysNoException($"Required property '{SignaturProperties.JobData}' not found for content type '{content.ContentType.Alias}'.");
        }

        bool modified = false;

        string? oldTitle = content.Name;
        string newTitle = item.Title;

        // Did the title change?
        if (oldTitle != newTitle) {
            // TODO: look for parentheses in the title as they can make it look like the title has changed
            content.Name = newTitle;
            modified = true;
        }

        // If the content item hasn't been created yet, we should make sure to set the job ID
        if (content.Id == 0) {
            content.SetValue(idProperty.Alias, item.WebAdId);
            modified = true;
        }

        // Has the data changed?
        string? oldData = isNew ? null : content.GetValue<string>(dataProperty.Alias);

        string newData = ToSource(item);
        SetValueIfModified(content, dataProperty.Alias, oldData, newData, ref modified);

        if (content.TryGetProperty(SignaturProperties.JobLastUpdated, out IProperty? lastUpdatedProperty)) {
            if (isNew) {
                content.SetValue(lastUpdatedProperty.Alias, $"_{EssentialsTime.UtcNow.Iso8601}");
            } else {
                string? value = content.GetValue<string>(lastUpdatedProperty.Alias);
                if (string.IsNullOrWhiteSpace(value)) {
                    modified = true;
                }
                content.SetValue(lastUpdatedProperty.Alias, $"_{EssentialsTime.UtcNow.Iso8601}");
            }
        }

        // Return whether the content item was modified
        return modified || content.Id == 0;

    }

    protected void SetValueIfModified<T>(IContent content, string propertyAlias, T? oldValue, T? newValue, ref bool modified) {
        if (Equals(oldValue, newValue)) return;
        content.SetValue(propertyAlias, newValue);
        modified = true;
    }

    protected string ToSource(ISignaturItem item) {

        if (item is SignaturItem si) {
            return SyndicationUtils.ToXmlString(si.Item);
        }

        throw new BjernerSaysNoException($"The type '{item.GetType()}' does not expose the underlying '{typeof(SyndicationItem)}'.");

    }

}