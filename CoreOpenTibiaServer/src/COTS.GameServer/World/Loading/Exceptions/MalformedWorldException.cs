namespace COTS.GameServer.World.Loading {

    public sealed class MalformedWorldException : WorldLoadingException {

        public MalformedWorldException()
           : base() { }

        public MalformedWorldException(string message)
            : base(message) { }
    }
}