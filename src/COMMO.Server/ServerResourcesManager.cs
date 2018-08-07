using System.IO;
using System.Reflection;

namespace COMMO.Server {
	public class ServerResourcesManager
    {
		public const string ItemsFilesDirectory = "COMMO.Server.Data.items";
		public const string MapFilesDirectory = "COMMO.Server.Data.map";
        public const string MapName = "COMMO.otbm";

		public static byte[] GetMap() {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(MapFilesDirectory + "." + MapName))
				return ReadFully(stream);
		}

		public static Stream GetItems(string itemsFileName) {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(ItemsFilesDirectory + "." + itemsFileName);
		}

		public static byte[] GetItemsBytes(string itemsFileName) {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(ItemsFilesDirectory + "." + itemsFileName))
				return ReadFully(stream);
		}

		private static byte[] ReadFully(Stream input)
		{
			using (var ms = new MemoryStream())
			{
				input.CopyTo(ms);
				return ms.ToArray();
			}
		}
    }
}
