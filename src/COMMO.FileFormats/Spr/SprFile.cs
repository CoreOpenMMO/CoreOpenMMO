//using COMMO.IO;
//using System.Collections.Generic;

//namespace COMMO.FileFormats.Spr
//{
//    public class SprFile
//    {
//        public static SprFile Load(string path)
//        {
//            using ( ByteArrayFileStream stream = new ByteArrayFileStream(path) )
//            {
//                ByteArrayStreamReader reader = new ByteArrayStreamReader(stream);

//                SprFile file = new SprFile();

//                file.signature = reader.ReadUInt();
            
//                ushort sprites = reader.ReadUShort();

//                file.sprites = new List<Sprite>(sprites);

//                for (ushort spriteId = 1; spriteId <= sprites; spriteId++)
//                {
//                    int index = reader.ReadInt();

//                    if (index > 0)
//                    {
//                        int returnIndex = stream.Position;

//                        stream.Seek(Origin.Begin, index);

//                            Sprite sprite = Sprite.Load(true, reader);

//                                sprite.Id = spriteId;

//                            file.sprites.Add(sprite);

//                            stream.Seek(Origin.Begin, returnIndex);
//                    }
//                }

//                return file;
//            }
//        }

//        private uint signature;

//        public uint Signature
//        {
//            get
//            {
//                return signature;
//            }
//        }

//        private List<Sprite> sprites;
        
//        public List<Sprite> Sprites
//        {
//            get
//            {
//                return sprites;
//            }
//        }
//    }
//}