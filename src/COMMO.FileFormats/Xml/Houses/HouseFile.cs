using System.Xml.Linq;
using System.Collections.Generic;

namespace COMMO.FileFormats.Xml.Houses
{
    public class HouseFile
    {
        public static HouseFile Load(string path)
        {
            var file = new HouseFile();

            file.Houses = new List<House>();

            foreach (var houseNode in XElement.Load(path).Elements("house") )
            {
                file.Houses.Add( House.Load(houseNode) );
            }

            return file;
        }

        public List<House> Houses { get; private set; }
    }
}