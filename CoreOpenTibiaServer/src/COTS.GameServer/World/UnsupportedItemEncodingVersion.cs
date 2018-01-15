namespace COTS.GameServer.World {

    public sealed class UnsupportedItemEncodingVersion : WorldLoadingException {

        public UnsupportedItemEncodingVersion()
           : base() { }

        public UnsupportedItemEncodingVersion(string message)
            : base(message) { }
    }
}