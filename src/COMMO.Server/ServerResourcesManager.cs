using System.IO;
using System.Reflection;

namespace COMMO.Server {
	public class ServerResourcesManager
    {
		private const string ItemsFilesDirectory = "COMMO.Server.Data.items";
		private const string MapFilesDirectory = "Data/map/COMMO.otbm";
		//private const string MapFilesDirectory = "Data/map/forgotten.otbm";

		public static string GetMap() => MapFilesDirectory;

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
