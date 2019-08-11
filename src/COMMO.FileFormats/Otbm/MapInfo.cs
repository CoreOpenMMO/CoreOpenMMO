using COMMO.IO;
using System.Collections.Generic;

namespace COMMO.FileFormats.Otbm
{
    public class MapInfo
    {
        public static MapInfo Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            var mapInfo = new MapInfo();

            stream.Seek(Origin.Current, 1);

            while (true)
            {
                switch ( (OtbmAttribute)reader.ReadByte() )
                {
                    case OtbmAttribute.Description:

                        mapInfo.Descriptions.Add( reader.ReadString() );

                        break;

                    case OtbmAttribute.SpawnFile:

                        mapInfo.SpawnFile = reader.ReadString();

                        break;

                    case OtbmAttribute.HouseFile:

                        mapInfo.HouseFile = reader.ReadString();

                        break;

                    default:

                        stream.Seek(Origin.Current, -1);

                        return mapInfo;
                }
            }
        }

        public List<string> Descriptions { get; } = new List<string>();

		public string SpawnFile { get; set; }

        public string HouseFile { get; set; }
    }
}