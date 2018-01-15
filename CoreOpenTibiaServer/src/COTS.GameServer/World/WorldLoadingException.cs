using System;

namespace COTS.GameServer.World {

    public class WorldLoadingException : Exception {

        public WorldLoadingException()
            : base() { }

        public WorldLoadingException(string message)
            : base(message) { }
    }
}