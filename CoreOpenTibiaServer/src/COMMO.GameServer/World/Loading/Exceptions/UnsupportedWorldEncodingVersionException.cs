namespace COMMO.GameServer.World.Loading {

    public sealed class UnsupportedWorldEncodingVersionException : WorldLoadingException {

        public UnsupportedWorldEncodingVersionException()
           : base() { }

        public UnsupportedWorldEncodingVersionException(string message)
            : base(message) { }
    }
}