using Newtonsoft.Json;

namespace Dialogflow {
    [JsonObject]
    public class Entity {
        [JsonProperty] public string Value { get; set; }

        [JsonProperty] public string[] Synonyms { get; set; }

        public Entity(string value, params string[] synonyms) {
            Value = value;
            Synonyms = synonyms;
        }
    }
}
