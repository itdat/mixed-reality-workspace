using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class EventInput {
        [JsonProperty] public string Name { get; set; }

        [JsonProperty] public Dictionary<string, object> Parameters { get; set; }

        [JsonProperty] public string LanguageCode { get; set; }
    }
}
