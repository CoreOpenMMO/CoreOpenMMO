using COMMO.Common.Structures;
using COMMO.IO;

namespace COMMO.FileFormats.Otbm
{
    public class Town
    {
        public static Town Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            var town = new Town();

            stream.Seek(Origin.Current, 1);

            town.Id = reader.ReadUInt();

            town.Name = reader.ReadString();

            town.Position = new Position(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte() );

            return town;
        }

        public uint Id { get; set; }

        public string Name { get; set; }

        public Position Position { get; set; }
    }
}