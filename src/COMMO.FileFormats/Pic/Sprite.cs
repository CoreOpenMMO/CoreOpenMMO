//using COMMO.IO;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.Runtime.InteropServices;

//namespace COMMO.FileFormats.Pic
//{
//    public class Sprite
//    {
//        public static Sprite Load(bool invisible, byte red, byte green, byte blue, ByteArrayStreamReader reader)
//        {
//            byte[] pixels = new byte[4 * 32 * 32];

//            int pixel = 0;


//            int length = reader.ReadUShort();

//            while (true)
//            {
//                if (length <= 0)
//                {
//                    break;
//                }

//                ushort transparent = reader.ReadUShort();

//                length -= 2;

//                if (invisible)
//                {
//                    pixel += transparent * 4;
//                }
//                else
//                {
//                    for (int i = 0; i < transparent; i++)
//                    {
//                        pixels[pixel++] = blue;

//                        pixels[pixel++] = green;

//                        pixels[pixel++] = red;

//                        pixels[pixel++] = 255;
//                    }
//                }

//                if (length <= 0)
//                {
//                    break;
//                }

//                ushort colored = reader.ReadUShort();

//                length -= 2;

//                byte[] coloredBytes = reader.ReadBytes(3 * colored);

//                length -= 3 * colored;

//                for (int i = 0; i < 3 * colored; i += 3)
//                {
//                    pixels[pixel++] = coloredBytes[i + 2];

//                    pixels[pixel++] = coloredBytes[i + 1];

//                    pixels[pixel++] = coloredBytes[i];

//                    pixels[pixel++] = 255;
//                }
//            }

//            Sprite sprite = new Sprite();

//            sprite.Pixels = pixels;

//            return sprite;
//        }

//        public byte[] Pixels { get; set; }
        
//        public Bitmap GetImage()
//        {
//            Bitmap bitmap = new Bitmap(32, 32);

//            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);

//            Marshal.Copy(Pixels, 0, bitmapData.Scan0, Pixels.Length);

//            bitmap.UnlockBits(bitmapData);
            
//            return bitmap;
//        }
//    }
//}