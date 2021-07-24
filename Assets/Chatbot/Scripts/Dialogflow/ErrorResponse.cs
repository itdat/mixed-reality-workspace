using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class ErrorResponse {
        [JsonProperty] public Error error { get; set; }
    }

    [JsonObject]
    public class Error {
        [JsonProperty] public long code { get; set; }

        [JsonProperty] public string message { get; set; }

        [JsonProperty] public string status { get; set; }
    }
}
