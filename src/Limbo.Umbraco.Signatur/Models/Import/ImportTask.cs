using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Time;

namespace Limbo.Umbraco.Signatur.Models.Import;

public class ImportTask {

    [JsonIgnore]
    public ImportTask? Parent { get; protected set; }

    [JsonIgnore]
    internal Stopwatch? Stopwatch;

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("duration")]
    [JsonConverter(typeof(TimeSpanSecondsConverter))]
    public TimeSpan? Duration { get; set; }

    [JsonProperty("exception")]
    public Exception? Exception { get; set; }

    [JsonProperty("status")]
    public ImportStatus Status { get; set; }

    [JsonProperty("action")]
    public ImportAction Action { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("items", Order = 998)]
    public List<ImportTask> Items = [];

    #region Constructors

    public ImportTask() { }

    public ImportTask(string? name) {
        Name = name;
    }

    #endregion

    #region Member methods

    public ImportTask AddTask(string name) {
        ImportTask item = new() { Name = name, Parent = this };
        Items.Add(item);
        return item;
    }

    public bool ShouldSerializeAction() {
        return Action != ImportAction.None;
    }

    public bool ShouldSerializeMessage() {
        return !string.IsNullOrWhiteSpace(Message);
    }

    #endregion

}