using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class QueryResult {
        [JsonProperty] public string queryText { get; set; }

        [JsonProperty] public string action { get; set; }

        [JsonProperty] public Dictionary<string, object> parameters { get; set; }

        [JsonProperty] public bool allRequiredParamsPresent { get; set; }

        [JsonProperty] public string fulfillmentText { get; set; }

        [JsonProperty] public Dictionary<string, object>[] fulfillmentMessages { get; set; }

        [JsonProperty] public Context[] outputContexts { get; set; }

        [JsonProperty] public Dictionary<string, object> intent { get; set; }

        [JsonProperty] public float intentDetectionConfidence { get; set; }

        [JsonProperty] public string languageCode { get; set; }
    }
}
