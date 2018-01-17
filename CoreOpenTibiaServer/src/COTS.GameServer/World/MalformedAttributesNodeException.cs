namespace COTS.GameServer.World {

    public sealed class MalformedAttributesNodeException : WorldLoadingException {

        public MalformedAttributesNodeException() {
        }

        public MalformedAttributesNodeException(string message)
            : base(message) { }
    }
}