using System;
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Factories;
using Limbo.Umbraco.Signatur.Models.Settings;
using Limbo.Umbraco.Signatur.Scheduling;
using Limbo.Umbraco.Signatur.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Strings;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Essentials.Time.Iso8601;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.Signatur.Composers;

public class SignaturComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<SignaturJobsService>();
        builder.Services.AddSingleton<ISignaturFeedParser, SignaturFeedParser>();
        builder.Services.AddSingleton<SignaturModelFactory>();
        builder.Services.AddOptions<SignaturSettings>().Configure<IConfiguration>(ConfigureSignatur);
        builder.Services.AddHostedService<SignaturRecurringTask>();
        builder.ManifestFilters().Append<SignaturManifestFilter>();
    }

    private static void ConfigureSignatur(SignaturSettings settings, IConfiguration configuration) {

        IConfigurationSection section = configuration.GetSection("Limbo:Signatur");

        settings.ImportUserId = StringUtils.ParseInt32(section.GetSection("ImportUserId")?.Value, settings.ImportUserId);
        settings.LogResults = StringUtils.ParseBoolean(section.GetSection("LogResults")?.Value, settings.LogResults);

        ParseFeeds(section, settings);
        ParseScheduling(section, settings);

    }

    private static void ParseFeeds(IConfiguration section, SignaturSettings settings) {

        IConfigurationSection? feeds = section.GetSection("Feeds");
        if (feeds == null) return;

        foreach (IConfigurationSection child in feeds.GetChildren()) {

            // Read from properties from their respective child sections
            string? url = child.GetSection("Url")?.Value;
            Guid? parentContentKey = child.GetSection("ParentContentKey")?.Value.ToGuid();
            string? contentTypeAlias = child.GetSection("ContentTypeAlias")?.Value;

            // Validate required properties
            if (string.IsNullOrWhiteSpace(url)) throw new Exception("Feed does not specify a URL.");
            if (parentContentKey is null || parentContentKey == Guid.Empty) throw new Exception("Feed does not specify a valid 'ParentContentKey' value.");
            if (string.IsNullOrWhiteSpace(contentTypeAlias)) throw new Exception("Feed does not specify a 'ContentTypeAlias' value.");

            // Append the item to the list
            settings.Feeds.Add(new SignaturFeedSettings(url, parentContentKey.Value, contentTypeAlias));

        }

    }

    private static void ParseScheduling(IConfiguration section, SignaturSettings settings) {

        IConfigurationSection? scheduling = section.GetSection("Scheduling");
        if (scheduling == null) return;

        string? delay = scheduling.GetSection("Delay")?.Value;
        string? interval = scheduling.GetSection("Interval")?.Value;

        if (int.TryParse(delay, out int delayMinutes)) {
            settings.Scheduling.Delay = TimeSpan.FromMinutes(delayMinutes);
        } else if (Iso8601Utils.TryParseDuration(delay, out TimeSpan delayTimeSpan)) {
            settings.Scheduling.Delay = delayTimeSpan;
        }

        if (int.TryParse(interval, out int internalMinutes)) {
            settings.Scheduling.Interval = TimeSpan.FromMinutes(internalMinutes);
        } else if (Iso8601Utils.TryParseDuration(interval, out TimeSpan internalTimeSpan)) {
            settings.Scheduling.Interval = internalTimeSpan;
        }

    }

}