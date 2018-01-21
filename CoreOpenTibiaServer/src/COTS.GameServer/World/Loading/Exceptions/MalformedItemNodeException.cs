namespace COTS.GameServer.World.Loading {

    public sealed class MalformedItemNodeException : WorldLoadingException {

        public MalformedItemNodeException()
            : base() { }

        public MalformedItemNodeException(string message)
            : base(message) { }
    }
}