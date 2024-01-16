using Umbraco.Cms.Core.Models;

namespace Limbo.Umbraco.Signatur.Models.Settings;

public class SignaturImportJobsSettings {

    public SignaturFeedSettings Feed { get; }

    public IContent Parent { get; }

    public IContentType ContentType { get; }

    public IPropertyType IdProperty { get; }

    public IPropertyType DataProperty { get; }

    public IPropertyType? LastUpdatedProperty { get; }

    public IPropertyType? TitleProperty { get; }

    public SignaturImportJobsSettings(SignaturFeedSettings feed, IContent parent,
        IContentType contentType, IPropertyType idProperty, IPropertyType dataProperty,
        IPropertyType? lastUpdatedProperty, IPropertyType? titleProperty) {
        Feed = feed;
        Parent = parent;
        ContentType = contentType;
        IdProperty = idProperty;
        DataProperty = dataProperty;
        LastUpdatedProperty = lastUpdatedProperty;
        TitleProperty = titleProperty;
    }

}