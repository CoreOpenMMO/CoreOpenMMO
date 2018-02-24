namespace COTS.GameServer.World.Loading {

    public sealed class MalformedWorldAttributesNodeException : WorldLoadingException {

        public MalformedWorldAttributesNodeException() {
        }

        public MalformedWorldAttributesNodeException(string message)
            : base(message) { }
    }
}