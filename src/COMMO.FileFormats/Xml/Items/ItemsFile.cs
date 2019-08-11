using System.Xml.Linq;
using System.Collections.Generic;

namespace COMMO.FileFormats.Xml.Items
{
    public class ItemsFile
    {
        public static ItemsFile Load(string path)
        {
            var file = new ItemsFile();

            file.Items = new List<Item>();

            foreach (var itemNode in XElement.Load(path).Elements("item") )
            {
                file.Items.Add( Item.Load(itemNode) );
            }

            return file;
        }

        public List<Item> Items { get; private set; }
    }
}