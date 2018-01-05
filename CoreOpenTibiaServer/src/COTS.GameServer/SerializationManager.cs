using COTS.GameServer.Vocations;
using Newtonsoft.Json;
using System;

namespace COTS.GameServer {

    public sealed class SerializationManager {

        public static JsonSerializerSettings JsonSerializerSettings {
            get {
                return new JsonSerializerSettings() {
                    Formatting = Formatting.Indented,
                    MissingMemberHandling = MissingMemberHandling.Error
                };
            }
        }

        public static string Serialize(Vocation vocation) {
            if (vocation == null)
                throw new ArgumentNullException(nameof(vocation));

            var serialized = JsonConvert.SerializeObject(
                value: vocation,
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