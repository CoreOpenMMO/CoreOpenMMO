using COMMO.IO;
using System.Collections.Generic;

namespace COMMO.FileFormats.Otbm
{
    public class OtbmFile
    {
        public static OtbmFile Load(string path)
        {
            using ( var stream = new ByteArrayFileTreeStream(path) )
            {
                var reader = new ByteArrayStreamReader(stream);
            
                var file = new OtbmFile();

                file.OtbmInfo = OtbmInfo.Load(stream, reader);

                if ( stream.Child() )
                {
                    file.MapInfo = MapInfo.Load(stream, reader);

                    if ( stream.Child() )
                    {
                        file.Areas = new List<Area>();

                        while(true)
                        {
                            switch ( (OtbmType)reader.ReadByte() )
                            {
                                case OtbmType.Area:

                                    file.Areas.Add( Area.Load(stream, reader) );

                                    break;

                                case OtbmType.Towns:

                                    if ( stream.Child() )
                                    {
                                        file.Towns = new List<Town>();

                                        while (true)
                                        {
                                            file.Towns.Add( Town.Load(stream, reader) );

                                            if ( !stream.Next() )
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    break;

                                case OtbmType.Waypoints:

                                    if ( stream.Child() )
                                    {
                                        file.Waypoints = new List<Waypoint>();

                                        while (true)
                                        {
                                            file.Waypoints.Add( Waypoint.Load(stream, reader) );

                                            if ( !stream.Next() )
                                            {
                                                break;
                                            }
                                        }                                
                                    }

                                    break;
                            }

                            if ( !stream.Next() )
                            {
                                break;
                            }
                        } 
                    }               
                }

                return file;      
            }
        }

        public OtbmInfo OtbmInfo { get; private set; }

        public MapInfo MapInfo { get; private set; }

        public List<Area> Areas { get; private set; }

        public List<Town> Towns { get; private set; }

        public List<Waypoint> Waypoints { get; private set; }
    }
}