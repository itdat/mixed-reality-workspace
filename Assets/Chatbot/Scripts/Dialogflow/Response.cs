using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class Response {
        [JsonProperty] public string responseId { get; set; }

        [JsonProperty] public QueryResult queryResult { get; set; }

        [JsonProperty] public string OutputAudio { get; set; }

        [JsonProperty] public OutputAudioConfig OutputAudioConfig { get; set; }
    }
}
