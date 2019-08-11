using COMMO.Common.Structures;
using System.Xml.Linq;

namespace COMMO.FileFormats.Xml.Npcs
{
    public class Npc
    {
        public static Npc Load(XElement npcNode)
        {
            var npc = new Npc();

            npc.Name = (string)npcNode.Attribute("name");

            npc.Speed = (ushort)(uint)npcNode.Attribute("speed");

            var healthNode = npcNode.Element("health");

            npc.Health = (ushort)(uint)healthNode.Attribute("now");

            npc.MaxHealth = (ushort)(uint)healthNode.Attribute("max");

            var outfitNode = npcNode.Element("look");

            npc.Outfit = new Outfit( (int)outfitNode.Attribute("type"), (int)outfitNode.Attribute("head"), (int)outfitNode.Attribute("body"), (int)outfitNode.Attribute("legs"), (int)outfitNode.Attribute("feet"), Addon.None );

            return npc;
        }

        public string Name { get; set; }

        public ushort Speed { get; set; }

        public ushort Health { get; set; }

        public ushort MaxHealth { get; set; }

        public Outfit Outfit { get; set; }        
    }
}