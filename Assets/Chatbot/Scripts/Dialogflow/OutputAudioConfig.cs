using Newtonsoft.Json;

//@hoatong
namespace Dialogflow {
    [JsonObject]
    public class OutputAudioConfig {
        [JsonProperty] public string AudioEncoding { get; set; }
    }
}
