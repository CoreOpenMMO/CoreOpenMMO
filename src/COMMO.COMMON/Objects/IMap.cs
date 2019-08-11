using COMMO.Common.Structures;

namespace COMMO.Common.Objects
{
    public interface IMap
    {
        Tile GetTile(Position position);
    }
}