using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class Request {
        [JsonProperty] public string Session { get; set; }

        [JsonProperty] public QueryInput QueryInput { get; set; }

        [JsonProperty] public DF2QueryParams QueryParams { get; set; }

        //@hoatong
        [JsonProperty] public OutputAudioConfig OutputAudioConfig { get; set; }

        [JsonProperty] public string InputAudio { get; set; }

        public Request(string session, QueryInput queryInput) {
            Session = session;
            QueryInput = queryInput;
        }
    }


    [JsonObject]
    public class DF2QueryParams {
        [JsonProperty] public Context[] Contexts { get; set; }

        [JsonProperty] public EntityType[] SessionEntityTypes { get; set; }
    }
}
