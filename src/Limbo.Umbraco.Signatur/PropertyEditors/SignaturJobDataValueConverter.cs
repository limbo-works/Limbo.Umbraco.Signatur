using System;
using System.ServiceModel.Syndication;
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Factories;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

public class SignaturJobDataValueConverter : PropertyValueConverterBase {

    private readonly ISignaturFeedParser _signaturFeedParser;
    private readonly SignaturModelFactory _signaturModelFactory;

    public SignaturJobDataValueConverter(ISignaturFeedParser signaturFeedParser, SignaturModelFactory signaturModelFactory) {
        _signaturFeedParser = signaturFeedParser;
        _signaturModelFactory = signaturModelFactory;
    }

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == SignaturJobDataEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {

        if (inter is not string xml) return null;

        // Parse the XML into a syndication item
        SyndicationItem syndicationItem = SyndicationUtils.FromXmlString(xml);

        // Parse the syndication item into a signatur item
        ISignaturItem item = _signaturFeedParser.ParseItem(syndicationItem);

        // Convert the signatur item into something else using the model factory. By default the signatur item will be
        // returned as is, but this can be controlled by overriding the model factory
        return _signaturModelFactory.ConvertJobData(owner, propertyType, item);

    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return _signaturModelFactory.GetJobDataValueType(propertyType);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}