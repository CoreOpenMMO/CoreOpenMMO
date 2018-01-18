using System;

namespace COTS.GameServer.World.Loading {

    public class WorldLoadingException : Exception {

        public WorldLoadingException()
            : base() { }

        public WorldLoadingException(string message)
            : base(message) { }
    }
}