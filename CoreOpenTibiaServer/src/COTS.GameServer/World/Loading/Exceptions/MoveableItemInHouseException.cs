namespace COTS.GameServer.World.Loading {

    public sealed class MoveableItemInHouseException : WorldLoadingException {

        public MoveableItemInHouseException()
            : base() { }

        public MoveableItemInHouseException(string message)
            : base(message) { }
    }
}