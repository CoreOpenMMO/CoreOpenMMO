using COMMO.Server.Data.Models.Structs;

namespace COMMO.Server {
	public class ServerResourcesManager
    {
		public static string ConfigVersion { get; set; }
		public static string ConfigMap { get; set; }

		public static Location StartLocation { get; set; }

		public static string GetMap() => $"Data/map/{ConfigVersion}/{ConfigMap}.otbm";
		public static string GetItemsOtb() => $"Data/items/{ConfigVersion}/items.otb";
		public static string GetItemsXml() => $"Data/items/{ConfigVersion}/items.xml";
	}

	
}
