using System;
using Skybrud.Essentials.Time;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

public class SignaturLastUpdatedValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == SignaturLastUpdatedEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source is string str ? str[1..] : null;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        return inter is string str ? EssentialsTime.FromIso8601(str) : null;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(EssentialsTime);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}