using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class TextInput {
        [JsonProperty] public string Text { get; set; }

        [JsonProperty] public string LanguageCode { get; set; }
    }
}
