namespace COTS.GameServer {

    public interface IThing {

        string GetDescription(uint lookDistance);

        Cylinder Parent { get; }
        Tile Tyle { get; }
    }
}