namespace COTS.GameServer.World.Loading {

    public sealed class MalformedTileNodeException : WorldLoadingException {

        public MalformedTileNodeException()
            : base() { }

        public MalformedTileNodeException(string message)
            : base(message) { }
    }
}