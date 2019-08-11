using COMMO.Common.Structures;
using System.Xml.Linq;

namespace COMMO.FileFormats.Xml.Items
{
    public class Item
    {
        public static Item Load(XElement itemNode)
        {
            var item = new Item();

            item.OpenTibiaId = (ushort)(uint)itemNode.Attribute("id");

            item.Article = (string)itemNode.Attribute("article");

            item.Name = (string)itemNode.Attribute("name");

            item.Plural = (string)itemNode.Attribute("plural");

            foreach (var attributeNode in itemNode.Elements("attribute") )
            {
                var key = attributeNode.Attribute("key");

                var value = attributeNode.Attribute("value");

                switch ( (string)key )
                {
                    case "article":

                        item.Article = (string)value;

                        break;

                    case "name":

                        item.Name = (string)value;

                        break;

                    case "plural":

                        item.Plural = (string)value;

                        break;

                    case "weight":

                        item.Weight = (uint)value;

                        break;

                    case "floorchange":

                        switch ( (string)value )
	                    {
		                    case "down":

                                item.FloorChange |= FloorChange.Down;

                                break;

                            case "north":

                                item.FloorChange |= FloorChange.North;

                                break;

                            case "south":

                                item.FloorChange |= FloorChange.South;

                                break;

                            case "west":

                                item.FloorChange |= FloorChange.West;

                                break;

                            case "east":

                                item.FloorChange |= FloorChange.East;

                                break;
	                    }
                        break;

                    case "containerSize":

                        item.ContainerSize = (byte)(int)value;

                        break;
                }
            }

            return item;
        }

        public ushort OpenTibiaId { get; set; }

        public string Article { get; set; }

        public string Name { get; set; }

        public string Plural { get; set; }

        public uint Weight { get; set; }

        public FloorChange FloorChange { get; set; }

        public byte ContainerSize { get; set; }
    }
}