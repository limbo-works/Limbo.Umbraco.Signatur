using System;
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Factories;
using Limbo.Umbraco.Signatur.Models.Settings;
using Limbo.Umbraco.Signatur.Scheduling;
using Limbo.Umbraco.Signatur.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Configuration;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.Signatur.Composers;

public class SignaturComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        builder.Services.AddSingleton<SignaturJobsService>();
        builder.Services.AddSingleton<ISignaturFeedParser, SignaturFeedParser>();
        builder.Services.AddSingleton<SignaturModelFactory>();
        builder.Services.AddSingleton<SignaturJobDataPropertyIndexValueFactory>();
        builder.Services.AddOptions<SignaturSettings>().Configure<IConfiguration>(ConfigureSignatur);

        if (builder.Config.GetBoolean("Limbo:Signatur:Scheduling:Enabled", true)) {
            builder.Services.AddHostedService<SignaturRecurringTask>();
        }

    }

    private static void ConfigureSignatur(SignaturSettings settings, IConfiguration configuration) {

        IConfigurationSection section = configuration.GetSection("Limbo:Signatur");

        settings.ImportUserId = section.GetInt32("ImportUserId", settings.ImportUserId);
        settings.LogResults = section.GetBoolean("LogResults", settings.LogResults);

        ParseFeeds(section, settings);
        ParseScheduling(section, settings);

    }

    private static void ParseFeeds(IConfiguration section, SignaturSettings settings) {

        IConfigurationSection feeds = section.GetSection("Feeds");

        foreach (IConfigurationSection child in feeds.GetChildren()) {

            // Read from properties from their respective child sections
            string? url = child.GetString("Url");
            Guid? parentContentKey = child.GetGuid("ParentContentKey");
            string? contentTypeAlias = child.GetString("ContentTypeAlias");

            // Validate required properties
            if (string.IsNullOrWhiteSpace(url)) throw new Exception("Feed does not specify a URL.");
            if (parentContentKey is null || parentContentKey == Guid.Empty) throw new Exception("Feed does not specify a valid 'ParentContentKey' value.");
            if (string.IsNullOrWhiteSpace(contentTypeAlias)) throw new Exception("Feed does not specify a 'ContentTypeAlias' value.");

            // Append the item to the list
            settings.Feeds.Add(new SignaturFeedSettings(url, parentContentKey.Value, contentTypeAlias));

        }

    }

    private static void ParseScheduling(IConfiguration section, SignaturSettings settings) {

        IConfigurationSection scheduling = section.GetSection("Scheduling");

        settings.Scheduling.Enabled = scheduling.GetSection("Enabled").Value.ToBoolean(true);
        settings.Scheduling.Delay = scheduling.GetTimeSpan("Delay", settings.Scheduling.Delay);
        settings.Scheduling.Interval = scheduling.GetTimeSpan("Interval", settings.Scheduling.Interval);

    }

}