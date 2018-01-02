using Newtonsoft.Json;
using System;

namespace SharpServer {

    public sealed class SerializationManager {

        public static JsonSerializerSettings JsonSerializerSettings {
            get {
                return new JsonSerializerSettings() {
                    Formatting = Formatting.Indented,
                    MissingMemberHandling = MissingMemberHandling.Error
                };
            }
        }

        public static string Serialize(Vocation knight) {
            if (knight == null)
                throw new ArgumentNullException(nameof(knight));

            var serialized = JsonConvert.SerializeObject(
                value: knight,
                settings: JsonSerializerSettings);

            return serialized;
        }

        public static Vocation DeserializeVocation(string serialized) {
            if (serialized == null)
                throw new ArgumentNullException(nameof(serialized));

            var deserialized = JsonConvert.DeserializeObject<Vocation>(
                value: serialized,
                settings: JsonSerializerSettings);
            return deserialized;
        }
    }
}