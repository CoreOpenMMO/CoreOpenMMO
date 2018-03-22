using System;

namespace COMMO.GameServer.World.TFSLoading {

    public class TFSWorldLoadingException : Exception {

        public TFSWorldLoadingException()
            : base() { }

        public TFSWorldLoadingException(string message)
            : base(message) { }
    }
}