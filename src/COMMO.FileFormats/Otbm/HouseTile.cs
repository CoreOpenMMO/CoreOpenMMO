using COMMO.IO;
using System.Collections.Generic;

namespace COMMO.FileFormats.Otbm
{
    public class HouseTile : Tile
    {
        public static HouseTile Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            var houseTile = new HouseTile();

            houseTile.OffsetX = reader.ReadByte();

            houseTile.OffsetY = reader.ReadByte();

            houseTile.HouseId = reader.ReadUInt();

            while (true)
            {
                switch ( (OtbmAttribute)reader.ReadByte() )
                {
                    case OtbmAttribute.Flags:

                        houseTile.Flags = (TileFlags)reader.ReadUInt();

                        break;

                    case OtbmAttribute.ItemId:

                        houseTile.ItemId = reader.ReadUShort();

                        break;

                    default:

                        stream.Seek(Origin.Current, -1);

                        if ( stream.Child() )
                        {
                            houseTile._items = new List<Item>();

                            while (true)
                            {
                                houseTile._items.Add( Item.Load(stream, reader) );

                                if ( !stream.Next() )
                                {
                                    break;
                                }
                            }
                        }
                        return houseTile;
                }
            }
        }

        public uint HouseId { get; set; }
    }
}