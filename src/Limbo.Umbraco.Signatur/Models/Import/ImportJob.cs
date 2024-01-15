using Newtonsoft.Json;

#pragma warning disable 1591

namespace Limbo.Umbraco.Signatur.Models.Import {

    public class ImportJob : ImportTask {

        [JsonProperty("type", Order = -999)]
        public static string Type => "Job";

        public ImportJob() { }

        public ImportJob(string name) {
            Name = name;
        }

    }

}