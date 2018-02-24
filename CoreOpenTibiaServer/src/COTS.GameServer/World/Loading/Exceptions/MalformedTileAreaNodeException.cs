namespace COTS.GameServer.World.Loading {

    public sealed class MalformedTileAreaNodeException : WorldLoadingException {

        public MalformedTileAreaNodeException()
            : base() { }

        public MalformedTileAreaNodeException(string message)
            : base(message) { }
    }
}