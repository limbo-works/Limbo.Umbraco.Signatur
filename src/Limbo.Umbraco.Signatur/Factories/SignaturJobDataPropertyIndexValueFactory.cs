using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using Limbo.Integrations.Signatur;
using Limbo.Umbraco.Signatur.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Limbo.Umbraco.Signatur.Factories;

public class SignaturJobDataPropertyIndexValueFactory : IPropertyIndexValueFactory {

    private readonly ISignaturFeedParser _signaturFeedParser;
    private readonly SignaturJobsService _signaturJobsService;

    public SignaturJobDataPropertyIndexValueFactory(ISignaturFeedParser signaturFeedParser, SignaturJobsService signaturJobsService) {
        _signaturFeedParser = signaturFeedParser;
        _signaturJobsService = signaturJobsService;
    }

    public IEnumerable<IndexValue> GetIndexValues(IProperty property, string? culture, string? segment, bool published, IEnumerable<string> availableCultures, IDictionary<Guid, IContentType> contentTypeDictionary) {
        // Get the source value from the property
        object? source = property.GetValue(culture, segment, published);

        // Validate the source value
        if (source is not string str || string.IsNullOrWhiteSpace(str)) yield break;

        // Add the property value (XML serialized string) to the index
        yield return new IndexValue{
            Culture = culture,
            FieldName = property.Alias,
            Values = [str]
        };

        // Parse the raw XMl into a 'SyndicationItem' instance
        SyndicationItem syndicationItem = SyndicationUtils.FromXmlString(str);

        // Parse the 'SyndicationItem' instance into a 'ISignaturItem' instance
        ISignaturItem signaturItem = _signaturFeedParser.ParseItem(syndicationItem);

        // TODO: Code smells a bit here ... can we optimize?
        foreach (var pair in _signaturJobsService.GetIndexValues(property, signaturItem, culture, segment, published)) {
            yield return new IndexValue{
                Culture = culture,
                FieldName = pair.Key,
                Values = pair.Value
            };
        }
    }
}