using System.Collections.Generic;

namespace Limbo.Umbraco.Signatur.Settings;

public class SignaturSettings {

    /// <summary>
    /// gets or sets the numeric ID of the backoffice user that should be set as responsible for the import actions (save and publish).
    /// </summary>
    public int ImportUserId { get; set; } = global::Umbraco.Cms.Core.Constants.Security.SuperUserId;

    /// <summary>
    /// Gets or sets a list of RSS feeds to be imported in Umbraco.
    /// </summary>
    public List<SignaturFeedSettings> Feeds { get; set; } = new();

}