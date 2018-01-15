namespace COTS.GameServer.World {

    public sealed class MalformedWorldHeaderNodeException : WorldLoadingException {

        public MalformedWorldHeaderNodeException()
           : base() { }

        public MalformedWorldHeaderNodeException(string message)
            : base(message) { }
    }
}