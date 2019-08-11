using COMMO.FileFormats.Otb;
using COMMO.IO;
using System;

namespace COMMO.FileFormats.Otbm
{
    public class OtbmInfo
    {
        public static OtbmInfo Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            var otbmInfo = new OtbmInfo();

            stream.Seek(Origin.Current, 6);

            otbmInfo.OtbmVersion = (OtbmVersion)reader.ReadUInt();

            otbmInfo.Width = reader.ReadUShort();

            otbmInfo.Height = reader.ReadUShort();

            otbmInfo.MajorVersion = (OtbVersion)reader.ReadUInt();

            otbmInfo.MinorVersion = (TibiaVersion)reader.ReadUInt();


            Console.WriteLine($"OTBM header version: {otbmInfo.OtbmVersion}.");
            Console.WriteLine($"World width: {otbmInfo.Width}.");
            Console.WriteLine($"World height: { otbmInfo.Height}.");
            Console.WriteLine($"Item encoding major version: { otbmInfo.MajorVersion}.");
            Console.WriteLine($"Item encoding minor version: { otbmInfo.MinorVersion}.");

            return otbmInfo;
        }

        public OtbmVersion OtbmVersion { get; set; }

        public ushort Width { get; set; }

        public ushort Height { get; set; }

        public OtbVersion MajorVersion { get; set; }

        public TibiaVersion MinorVersion { get; set; }
    }
}