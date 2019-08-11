using COMMO.IO;

namespace COMMO.FileFormats.Otb
{
    public class OtbInfo
    {
        public static OtbInfo Load(ByteArrayFileTreeStream stream, ByteArrayStreamReader reader)
        {
            var otbInfo = new OtbInfo();

            stream.Seek(Origin.Current, 13);

            otbInfo.MajorVersion = (OtbVersion)reader.ReadUInt();

            otbInfo.MinorVersion = (TibiaVersion)reader.ReadUInt();

            otbInfo.Revision = reader.ReadUInt();

            otbInfo.CsdVersion = reader.ReadCsd();

            return otbInfo;
        }

        public OtbVersion MajorVersion { get; set; }

        public TibiaVersion MinorVersion { get; set; }

        public uint Revision { get; set; }

        public string CsdVersion { get; set; }
    }
}