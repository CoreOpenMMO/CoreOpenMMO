using COMMO.Common.Structures;
using System.Xml.Linq;

namespace COMMO.FileFormats.Xml.Houses
{
    public class House
    {
        public static House Load(XElement houseNode)
        {
            var house = new House();

            house.Id = (uint)houseNode.Attribute("houseid");
            
            house.Name = (string)houseNode.Attribute("name");

            house.Entry = new Position((int)houseNode.Attribute("entryx"), (int)houseNode.Attribute("entryy"), (int)houseNode.Attribute("entryz") );

            house.TownId = (uint)houseNode.Attribute("townid");

            house.Rent = (uint)houseNode.Attribute("rent");
            
            house.Size = (uint)houseNode.Attribute("size");

            return house;
        }

        public uint Id { get; set; }

        public string Name { get; set; }

        public Position Entry { get; set; }

        public uint TownId { get; set; }

        public uint Rent { get; set; }

        public uint Size { get; set; }
    }
}