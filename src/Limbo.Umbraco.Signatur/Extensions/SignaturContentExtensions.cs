using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Umbraco.Cms.Core.Models;

namespace Limbo.Umbraco.Signatur.Extensions;

internal static class SignaturContentExtensions {

    // TODO: Move to "Skybrud.Essentials.Umbraco"

    public static bool HasProperty(this IContent content, string propertyAlias) {
        return content.Properties.FirstOrDefault(x => x.Alias == propertyAlias) != null;
    }

    public static bool TryGetProperty(this IContent content, string propertyAlias, [NotNullWhen(true)] out IProperty? result) {

        result = content.Properties.FirstOrDefault(x => x.Alias == propertyAlias);
        return result != null;

    }

}