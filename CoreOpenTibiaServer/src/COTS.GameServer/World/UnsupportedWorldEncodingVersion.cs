namespace COTS.GameServer.World {

    public sealed class UnsupportedWorldEncodingVersion : WorldLoadingException {

        public UnsupportedWorldEncodingVersion()
           : base() { }

        public UnsupportedWorldEncodingVersion(string message)
            : base(message) { }
    }
}