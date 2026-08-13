using System;
using Skybrud.Essentials.Time;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.PropertyEditors;

public class SignaturLastUpdatedValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == SignaturLastUpdatedPropertyEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        if (source is not string str || string.IsNullOrWhiteSpace(str)) return null;
        return str.StartsWith("_") ? str[1..] : str;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        return inter is string str && !string.IsNullOrWhiteSpace(str) ? EssentialsTime.FromIso8601(str) : null;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(EssentialsTime);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}