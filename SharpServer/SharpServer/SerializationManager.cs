using System;
using System.IO;

namespace SharpServer {

    public sealed class SerializationManager {
        public readonly string DataDirectoryAbsolutePath;

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
    }
}