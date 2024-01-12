using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Limbo.Umbraco.Signatur;

/// <summary>
/// Static class with various utility methods for working with <see cref="SyndicationFeed"/> and <see cref="SyndicationItem"/>.
/// </summary>
/// <remarks>
/// This class is marked as internal as we probably possible maybe should release this as a standalone package, and then add it as a dependency here.
/// </remarks>
internal static class SyndicationUtils {

    public static string ToXmlString(SyndicationItem item) {

        StringBuilder sb = new();

        XmlWriterSettings settings = new() {
            OmitXmlDeclaration = true,
            Indent = true
        };

        using (XmlWriter writer = XmlWriter.Create(sb, settings)) {
            item.GetRss20Formatter().WriteTo(writer);
        }

        return sb.ToString();

    }

    public static SyndicationItem FromXmlString(string xml) {

        using TextReader tr = new StringReader(xml);

        using XmlReader reader = XmlReader.Create(tr);

        return SyndicationItem.Load(reader);

    }

    public static Dictionary<string, string> ToDictionary(SyndicationItem item) {
        return item.ElementExtensions
            .ToDictionary(x => x.OuterName, x => x
                .GetReader()
                .ReadElementContentAsString()
            );
    }

}