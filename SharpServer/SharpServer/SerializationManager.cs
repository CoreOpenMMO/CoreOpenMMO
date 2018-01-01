using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace SharpServer {

    public sealed class SerializationManager {
        public const string VocationsFilename = "Vocations.Json";

        public readonly string DataDirectoryAbsolutePath;

        public string VocationsFileFullPath => Path.Combine(DataDirectoryAbsolutePath, VocationsFileFullPath);

        public SerializationManager(string dataDirectoryPath) {
            if (dataDirectoryPath == null)
                throw new ArgumentNullException(nameof(dataDirectoryPath));

            var dataDirectoryAbsolutePath = Path.GetFullPath(dataDirectoryPath);
            if (!Directory.Exists(dataDirectoryAbsolutePath)) {
                var exceptionMessage = $"Argument {nameof(dataDirectoryPath)} was translated to absolute path that doesn't exist: {{{dataDirectoryAbsolutePath}}}";
                throw new ArgumentException(exceptionMessage);
            }

            DataDirectoryAbsolutePath = dataDirectoryAbsolutePath;
        }

        public ReadOnlyArray<Vocation> LoadVocations() {
            if (!File.Exists(VocationsFileFullPath))
                throw new FileNotFoundException(VocationsFileFullPath);

            var serializedVocations = File.ReadAllText(VocationsFileFullPath);
            var deserializedVocatiosn = JsonConvert.DeserializeObject<ReadOnlyArray<Vocation>>(serializedVocations);

            return deserializedVocatiosn;
        }

        public void SerializeVocations(ReadOnlyArray<Vocation> vocations) {
            if (vocations == null)
                throw new ArgumentNullException(nameof(vocations));

            var serializedVocations = JsonConvert.SerializeObject(
                value: vocations,
                formatting: Formatting.Indented);
            File.WriteAllText(
                path: VocationsFileFullPath,
                contents: serializedVocations,
                encoding: Encoding.UTF8);
        }
    }
}