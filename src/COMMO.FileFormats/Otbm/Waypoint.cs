using COMMO.Common.Structures;
using COMMO.IO;

namespace COMMO.FileFormats.Otbm
{
    public class Waypoint
    {
        public static Waypoint Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            var waypoint = new Waypoint();

            stream.Seek(Origin.Current, 1);

            waypoint.Name = reader.ReadString();

            waypoint.Position = new Position(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte() );

            return waypoint;
        }

        public string Name { get; set; }

        public Position Position { get; set; }
    }
}