using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Models.Import;
using Limbo.Umbraco.Signatur.Models.Settings;
using Limbo.Umbraco.Signatur.PropertyEditors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Skybrud.Essentials.Common;
using Skybrud.Essentials.Exceptions;
using Skybrud.Essentials.Time;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace Limbo.Umbraco.Signatur.Services;

public class SignaturJobsService {

    private readonly SignaturSettings _settings;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IContentService _contentService;
    private readonly ISignaturFeedParser _signaturFeedParser;

    public SignaturJobsService(IOptions<SignaturSettings> settings, IWebHostEnvironment webHostEnvironment, IContentService contentService, ISignaturFeedParser signaturFeedParser) {
        _settings = settings.Value;
        _webHostEnvironment = webHostEnvironment;
        _contentService = contentService;
        _signaturFeedParser = signaturFeedParser;
    }

    public ImportJob Import(SignaturFeedSettings feed) {

        // Validate the feed
        if (string.IsNullOrWhiteSpace(feed.Url)) throw new PropertyNotSetException(nameof(feed.Url));
        if (feed.ParentContentKey == Guid.Empty) throw new PropertyNotSetException(nameof(feed.ParentContentKey));
        if (string.IsNullOrWhiteSpace(feed.ContentTypeAlias)) throw new PropertyNotSetException(nameof(feed.ContentTypeAlias));

        ImportJob job = new ImportJob($"Importing job items from {feed.Url}").Start();

        // Get the parent content node
        ImportTask task1 = job.AddTask($"Getting parent node by key '{feed.ParentContentKey}'...").Start();
        IContent? parent = _contentService.GetById(feed.ParentContentKey);
        if (parent is null) {
            task1.AppendToMessage($"Parent with key '{feed.ParentContentKey}' not found.").Failed();
            return job;
        } else {
            task1.AppendToMessage($"Found content node with name '{parent.Name}'...").Completed();
        }

        #region Get existing content/jobs

        ImportTask task2 = job.AddTask("Getting erxisting jobs from content service...").Start();

        Dictionary<int, IContent> existing = new();

        try {

            foreach (IContent content in _contentService.GetPagedChildren(parent.Id, 0, int.MaxValue, out long _)) {

                if (content.ContentType.Alias != feed.ContentTypeAlias) continue;

                int jobId = content.GetValue<int>("signaturJobId");
                if (jobId == 0) continue;

                existing[jobId] = content;

            }

            task2.AppendToMessage($"Found {existing.Count} jobs...").Completed();

        } catch (Exception ex) {

            task2.Failed(ex);

            return job;

        }

        #endregion

        #region Get the jobs from the RSS feed

        ImportTask task3 = job.AddTask("Getting jobs from Signatur RSS feed...").Start();

        ISignaturFeed rssFeed;
        try {
            rssFeed = _signaturFeedParser.Load(feed.Url);
            task3.AppendToMessage($"Found {rssFeed.Items.Count} job items...").Completed();
        } catch (Exception ex) {
            task3.Failed(ex);
            return job;
        }

        #endregion

        #region Add or update the jobs in Umbraco

        ImportTask task4 = job.AddTask("Importing tasks in Umbraco...").Start();

        try {

            foreach (ISignaturItem item in rssFeed.Items) {
                AddOrUpdate(item, parent, feed, task4, existing);
            }

            task4.Completed();

            if (task4.Status == ImportStatus.Failed) return job;

        } catch (Exception ex) {

            task4.Failed(ex);
            return job;

        }

        #endregion

        #region Delete jobs no longer in the feed

        ImportTask task5 = job.AddTask("Deleting jobs in Umbraco...").Start();

        try {

            // TODO: Delete jobs no longer in the feed

            task5.AppendToMessage("Not yet implemented :'(").Failed();

        } catch (Exception ex) {

            task5.Failed(ex);

            return job;

        }

        #endregion

        return job.Completed();

    }

    protected virtual void AddOrUpdate(ISignaturItem item, IContent parent, SignaturFeedSettings feed, ImportTask parentTask, Dictionary<int, IContent> existing) {

        ImportTask task = parentTask.AddTask($"Import job item with name '{item.Title}' and ID '{item.WebAdId}'...").Start();

        try {

            bool isNew = false;

            // If the content doesn't already exist, we create in
            if (!existing.TryGetValue(item.WebAdId, out IContent? content)) {
                content = _contentService.Create(item.Title, parent, feed.ContentTypeAlias, _settings.ImportUserId);
                isNew = true;
                task.AppendToMessage("Job item not found in Umbraco. Creating new content item...");
            } else {
                task.AppendToMessage($"Job item found in Umbraco with ID '{content.Id}'...");
            }

            // Update the Umbraco properties based on the job item
            bool modified = UpdateProperties(item, content, task, content.Id == 0);

            // Save and published the content item if we detecthed any changes
            if (modified) {
                _contentService.SaveAndPublish(content, userId: _settings.ImportUserId);
                if (isNew) {
                    task.AppendToMessage($"Successfully created and published content item with ID '{content.Id}'...").SetAction(ImportAction.Added);
                } else {
                    task.AppendToMessage("Successfully saved and published changes...").SetAction(ImportAction.Updated);
                }
            } else if (!isNew) {
                task.AppendToMessage("No new changed detected for job item. Skipping for now...").SetAction(ImportAction.NotModified);
            }

            existing.Remove(item.WebAdId);

            task.Completed();

        } catch (Exception ex) {

            task.Failed(ex);

        }

    }

    protected virtual bool UpdateProperties(ISignaturItem item, IContent content, ImportTask task, bool isNew) {

        IProperty? idProperty = null;
        IProperty? dataProperty = null;
        IProperty? lastUpdatedProperty = null;

        foreach (IProperty property in content.Properties) {

            switch (property.PropertyType.PropertyEditorAlias) {

                case SignaturJobIdEditor.EditorAlias:
                    idProperty = property;
                    break;

                case SignaturJobDataEditor.EditorAlias:
                    dataProperty = property;
                    break;

                case SignaturLastUpdatedEditor.EditorAlias:
                    lastUpdatedProperty = property;
                    break;

            }

        }

        if (idProperty is null) throw new BjernerSaysNoException($"Required property with property editor '{SignaturJobIdEditor.EditorAlias}' not found for content type '{content.ContentType.Alias}'.");
        if (dataProperty is null) throw new BjernerSaysNoException($"Required property with property editor '{SignaturJobDataEditor.EditorAlias}' not found for content type '{content.ContentType.Alias}'.");

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

        if (lastUpdatedProperty is not null) {
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

    /// <summary>
    /// Writes the log of the specified <paramref name="job"/> to the disk.
    /// </summary>
    /// <param name="job">The job.</param>
    public virtual void WriteToLog(ImportJob job) {

        string path = Path.Combine(global::Umbraco.Cms.Core.Constants.SystemDirectories.LogFiles, SignaturPackage.Alias, $"{DateTime.UtcNow:yyyyMMddHHmmss}.txt");

        string fullPath = _webHostEnvironment.MapPathContentRoot(path);

        // ReSharper disable once AssignNullToNotNullAttribute
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        System.IO.File.AppendAllText(fullPath, JsonConvert.SerializeObject(job), Encoding.UTF8);

    }

}