namespace COTS.GameServer.World.Loading {

    public sealed class MalformedAttributesNodeException : WorldLoadingException {

        public MalformedAttributesNodeException() {
        }

        public MalformedAttributesNodeException(string message)
            : base(message) { }
    }
}