//using System.Drawing;
//using System.Collections.Generic;
//using COMMO.IO;

//namespace COMMO.FileFormats.Pic
//{
//    public class Image
//    {
//        public static Image Load(ByteArrayFileStream stream, ByteArrayStreamReader reader)
//        {
//            Image image = new Image();

//            image.Width = reader.ReadByte();

//            image.Height = reader.ReadByte();

//            image.Red = reader.ReadByte();

//            image.Green = reader.ReadByte();

//            image.Blue = reader.ReadByte();

//            int sprites = image.Width * image.Height;

//            image.sprites = new List<Sprite>(sprites);

//            for (int i = 0; i < sprites; i++)
//            {
//                int index = reader.ReadInt();

//                if (index > 0)
//                {
//                    int returnIndex = stream.Position;

//                    stream.Seek(Origin.Begin, index);

//                        Sprite sprite = Sprite.Load(true, image.Red, image.Green, image.Blue, reader);

//                        image.sprites.Add(sprite);

//                        stream.Seek(Origin.Begin, returnIndex);
//                }
//            }

//            return image;
//        }

//        public ushort Id { get; set; }

//        public byte Width { get; set; }

//        public byte Height { get; set; }

//        public byte Red { get; set; }

//        public byte Green { get; set; }

//        public byte Blue { get; set; }

//        private List<Sprite> sprites;

//        public List<Sprite> Sprites
//        {
//            get
//            {
//                return sprites;
//            }
//        }

//        public Bitmap GetImage()
//        {
//            Bitmap bitmap = new Bitmap(32 * Width, 32 * Height);

//            using ( Graphics graphics = Graphics.FromImage(bitmap) )
//            {
//                int index = 0;

//                for (int j = 0; j < Height; j++)
//                {
//                    for (int i = 0; i < Width; i++)
//                    {                       
//                        graphics.DrawImage(sprites[index++].GetImage(), 32 * i, 32 * j);
//                    }
//                }
//            }
//            return bitmap;
//        }
//    }
//}