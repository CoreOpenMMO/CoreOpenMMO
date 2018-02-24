namespace COTS.GameServer.World.Loading {

    public sealed class MalformedWorldHeaderNodeException : WorldLoadingException {

        public MalformedWorldHeaderNodeException()
           : base() { }

        public MalformedWorldHeaderNodeException(string message)
            : base(message) { }
    }
}