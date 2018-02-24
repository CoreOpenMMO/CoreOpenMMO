namespace COTS.GameServer.World.Loading {

    public sealed class UnsupportedItemEncodingVersionException : WorldLoadingException {

        public UnsupportedItemEncodingVersionException()
           : base() { }

        public UnsupportedItemEncodingVersionException(string message)
            : base(message) { }
    }
}