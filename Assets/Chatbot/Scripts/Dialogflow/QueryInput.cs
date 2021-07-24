using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class QueryInput {
        public TextInput Text { get; set; }

        public EventInput Event { get; set; }

        public AudioConfig AudioConfig { get; set; }
    }
}
