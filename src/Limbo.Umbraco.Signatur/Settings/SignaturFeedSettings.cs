using System;

namespace Limbo.Umbraco.Signatur.Settings;

public class SignaturFeedSettings {

    public string Url { get; }

    public Guid ParentContentKey { get; }

    public string ContentTypeAlias { get; }

    public SignaturFeedSettings(string url, string parentContentKey, string contentTypeAlias) {
        Url = url;
        if (!Guid.TryParse(parentContentKey, out Guid parentContentKeyGuid)) throw new ArgumentException("Value is not a valid GUID.", nameof(parentContentKeyGuid));
        ParentContentKey = parentContentKeyGuid;
        ContentTypeAlias = contentTypeAlias;
    }

    public SignaturFeedSettings(string url, Guid parentContentKey, string contentTypeAlias) {
        Url = url;
        ParentContentKey = parentContentKey;
        ContentTypeAlias = contentTypeAlias;
    }

}