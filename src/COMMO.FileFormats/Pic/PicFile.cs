//using COMMO.IO;
//using System.Collections.Generic;

//namespace COMMO.FileFormats.Pic
//{
//    public class PicFile
//    {
//        public static PicFile Load(string path)
//        {
//            using ( ByteArrayFileStream stream = new ByteArrayFileStream(path) )
//            {
//                ByteArrayStreamReader reader = new ByteArrayStreamReader(stream);

//                PicFile file = new PicFile();

//                file.signature = reader.ReadUInt();

//                ushort images = reader.ReadUShort();

//                file.images = new List<Image>(images);

//                for (ushort imageId = 0; imageId < images; imageId++)
//                {
//                    Image image = Image.Load(stream, reader);

//                        image.Id = imageId;

//                    file.images.Add(image);
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

//        private List<Image> images;
        
//        public List<Image> Images
//        {
//            get
//            {
//                return images;
//            }
//        }
//    }
//}