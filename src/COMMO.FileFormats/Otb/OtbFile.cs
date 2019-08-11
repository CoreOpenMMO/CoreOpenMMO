using COMMO.IO;
using System.Collections.Generic;

namespace COMMO.FileFormats.Otb
{
    public class OtbFile
    {
        public static OtbFile Load(string path)
        {
            using ( var stream = new ByteArrayFileTreeStream(path) )
            {
                var reader = new ByteArrayStreamReader(stream);
            
                var file = new OtbFile();

                file.OtbInfo = OtbInfo.Load(stream, reader);

                if ( stream.Child() )
                {
                    file.Items = new List<Item>();

                    while (true)
                    {
                        file.Items.Add( Item.Load(stream, reader) );

                        if ( !stream.Next() )
                        {
                            break;
                        }
                    }
                }

                return file;  
            }
        }

        public OtbInfo OtbInfo { get; private set; }

        public List<Item> Items { get; private set; }
    }
}