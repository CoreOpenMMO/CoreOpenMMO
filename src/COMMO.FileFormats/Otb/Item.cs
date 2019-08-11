using COMMO.Common.Objects;
using COMMO.Common.Structures;
using COMMO.IO;

namespace COMMO.FileFormats.Otb
{
    public class Item
    {
        public static Item Load(ByteArrayFileTreeStream stream , ByteArrayStreamReader reader)
        {
            var item = new Item();

            item.Group = (ItemGroup)reader.ReadByte();

            item.Flags = (ItemFlags)reader.ReadUInt();

            while (true)
            {
                var attribute = (OtbAttribute)reader.ReadByte();

                stream.Seek(Origin.Current, 2);

                switch (attribute)
                {
                    case OtbAttribute.OpenTibiaId:

                        item.OpenTibiaId = reader.ReadUShort();

                        break;

                    case OtbAttribute.TibiaId:

                        item.TibiaId = reader.ReadUShort();

                        break;

                    case OtbAttribute.Speed:

                        item.Speed = reader.ReadUShort();

                        break;

                    case OtbAttribute.SpriteHash:

                        item.SpriteHash = reader.ReadBytes(16);

                        break;

                    case OtbAttribute.MinimapColor:

                        item.MinimapColor = reader.ReadUShort();

                        break;

                    case OtbAttribute.MaxReadWriteChars:

                        item.MaxReadWriteChars = reader.ReadUShort();

                        break;

                    case OtbAttribute.MaxReadChars:

                        item.MaxReadChars = reader.ReadUShort();

                        break;

                    case OtbAttribute.Light:

                        item.LightLevel = reader.ReadUShort();

                        item.LightColor = reader.ReadUShort();

                        break;

                    case OtbAttribute.TopOrder:

                        item.TopOrder = (TopOrder)reader.ReadByte();

                        break;

                    default:

                        stream.Seek(Origin.Current, -3);

                        return item;
                }
            }
        }

        public ItemGroup Group { get; set; }

        public ItemFlags Flags { get; set; }

        public ushort OpenTibiaId { get; set; }

        public ushort TibiaId { get; set; }

        public ushort Speed { get; set; }

        public byte[] SpriteHash { get; set; }

        public ushort MinimapColor { get; set; }

        public ushort MaxReadWriteChars { get; set; }

        public ushort MaxReadChars { get; set; }

        public ushort LightLevel { get; set; }

        public ushort LightColor { get; set; }

        public TopOrder TopOrder { get; set; }
    }
}