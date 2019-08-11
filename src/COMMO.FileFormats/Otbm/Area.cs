using COMMO.Common.Structures;
using COMMO.IO;
using System.Collections.Generic;

namespace COMMO.FileFormats.Otbm
{
    public class Area
    {
        public static Area Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            var area = new Area();

            area.Position = new Position(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte() );

            if ( stream.Child() )
            {
                area.Tiles = new List<Tile>(1);

                while (true)
                {
                    Tile tile = null;

                    switch ( (OtbmType)reader.ReadByte() )
                    {
                        case OtbmType.Tile:

                            tile = Tile.Load(stream, reader);

                            break;

                        case OtbmType.HouseTile:

                            tile = HouseTile.Load(stream, reader);

                            break;
                    }

                    area.Tiles.Add(tile);
                    
                    if ( !stream.Next() )
                    {
                        break; 
                    }
                }
            }

            return area;
        }

        public Position Position { get; set; }

		public List<Tile> Tiles { get; private set; }
    }
}