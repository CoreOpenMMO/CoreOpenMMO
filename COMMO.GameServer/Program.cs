using COMMO.GameServer.Lua;
using COMMO.GameServer.Network;
using COMMO.GameServer.Network.Protocols;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace COMMO.GameServer {

	public sealed class Program {

		private static ServiceProvider _serviceProvider;

		private static void Main(string[] args) {

			var serviceCollection = new ServiceCollection();
			ConfigureLocalServices(serviceCollection);

			_serviceProvider = serviceCollection.BuildServiceProvider();

			Console.ReadLine();
		}

		public static void ConfigureLocalServices(IServiceCollection serviceCollection) {
			serviceCollection.AddTransient<LuaManager>();
			serviceCollection.AddTransient<ConnectionManager>();
			serviceCollection.AddTransient<LoginProtocol>();
			serviceCollection.AddTransient<GameProtocol>();
		}
	}
}
