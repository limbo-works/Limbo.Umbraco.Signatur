using System;
using System.ServiceModel.Syndication;
using Limbo.Integrations.Signatur;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors; 

public class SignaturJobDataValueConverter : PropertyValueConverterBase {

    private readonly ISignaturFeedParser _signaturFeedParser;

    public SignaturJobDataValueConverter(ISignaturFeedParser signaturFeedParser) {
        _signaturFeedParser = signaturFeedParser;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == SignaturJobDataEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {

        if (inter is not string xml) return null;

        SyndicationItem syndicationItem = SyndicationUtils.FromXmlString(xml);

        return _signaturFeedParser.ParseItem(syndicationItem);

    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return _signaturFeedParser.ItemType;
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}