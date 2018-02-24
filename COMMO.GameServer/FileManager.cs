using System.IO;

namespace COMMO.GameServer
{
    public static class FileManager
    {
        public enum FileType{ITEMS_OTB, ITEMS_XML, MAP};
        public enum Dir{DATA, ITEMS, MAP};

        public static string BaseDirectory = Directory.GetParent(
            Directory.GetParent(
                Directory.GetCurrentDirectory()
            ).ToString()
        ).ToString() + "/";

        public static string GetDirectory(Dir dir)
        {
            switch (dir)
            {
                case Dir.DATA:
                    return BaseDirectory + "Data/"; // Should get from Config file or argline
                case Dir.ITEMS:
                    return GetDirectory(Dir.DATA) + "Items/";
                case Dir.MAP:
                    return GetDirectory(Dir.DATA) + "Map/";
            }

            return "";
        }

        public static string GetFilePath(FileType file)
        {
            switch (file)
            {
                case FileType.ITEMS_OTB:
                    return GetDirectory(Dir.ITEMS) + "Items.otb";
                case FileType.ITEMS_XML:
                    return GetDirectory(Dir.ITEMS) + "Items.xml";
                case FileType.MAP:
                    return GetDirectory(Dir.MAP) + "MapName.otbm"; // Should get from Config file or argline
            }

            return "";
        }

        public static byte[] ReadFileToByteArray(string path, bool ignoreException = false)
        {
            byte[] data = null;
            try
            {
                data = File.ReadAllBytes(path);
            }
            catch (FileNotFoundException)
            {
                if(ignoreException)
                    return null;
                else
                    throw new FileNotFoundException();
            }

            return data;
        }

        public static byte[] ReadFileToByteArray(FileType file)
        {
            return ReadFileToByteArray(GetFilePath(file));
        }

        public static byte[] ReadFileToByteArray(Dir dir, string filename)
        {
            return ReadFileToByteArray(GetDirectory(dir) + filename);
        }
    }
}