using System;

namespace Limbo.Umbraco.Signatur.Models.Settings;

public class SignaturSchedulingSettings {

    /// <summary>
    /// Gets or sets whether the import should run in a background task. Default is <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the initial delay from startup and until the job is run the first time. Default <strong>5 minutes</strong>.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the interval between each run. Default is <strong>1 hour</strong>.
    /// </summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);

}