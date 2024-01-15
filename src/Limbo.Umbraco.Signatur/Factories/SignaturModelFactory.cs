using System;
using Limbo.Integrations.Signatur;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace Limbo.Umbraco.Signatur.Factories;

public class SignaturModelFactory {

    private readonly ISignaturFeedParser _signaturFeedParser;

    public SignaturModelFactory(ISignaturFeedParser signaturFeedParser) {
        _signaturFeedParser = signaturFeedParser;
    }

    public virtual Type GetJobDataValueType(IPublishedPropertyType propertyType) {
        return _signaturFeedParser.ItemType;
    }

    public virtual object ConvertJobData(IPublishedElement owner, IPublishedPropertyType propertyType, ISignaturItem item) {
        return item;
    }

}