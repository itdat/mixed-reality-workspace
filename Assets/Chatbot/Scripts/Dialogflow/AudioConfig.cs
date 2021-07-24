using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class AudioConfig {
        [JsonProperty] public string LanguageCode { get; set; }
    }
}
