using System;
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Models.Settings;
using Limbo.Umbraco.Signatur.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Limbo.Umbraco.Signatur.Composers;

public class SignaturComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {
        builder.Services.AddSingleton<SignaturJobsService>();
        builder.Services.AddSingleton<ISignaturFeedParser, SignaturFeedParser>();
        builder.Services.AddOptions<SignaturSettings>().Configure<IConfiguration>(ConfigureSignatur);
        builder.ManifestFilters().Append<SignaturManifestFilter>();
    }

    private static void ConfigureSignatur(SignaturSettings settings, IConfiguration configuration) {

        IConfigurationSection section = configuration.GetSection("Limbo:Signatur");

        IConfigurationSection? feeds = section?.GetSection("Feeds");
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

}