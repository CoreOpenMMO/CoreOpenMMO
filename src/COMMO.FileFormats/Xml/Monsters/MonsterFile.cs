using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace COMMO.FileFormats.Xml.Monsters
{
    public class MonsterFile
    {
        public static MonsterFile Load(string path)
        {
            var file = new MonsterFile();

            file.Monsters = new List<Monster>();

            if (Directory.Exists(path) )
            {
                foreach (var directoryName in Directory.GetDirectories(path) )
                {
                    foreach (var fileName in Directory.GetFiles(directoryName) )
                    {
                        file.Monsters.Add( Monster.Load( XElement.Load(fileName) ) );
                    }
                }
            }

            return file;
        }

        public List<Monster> Monsters { get; private set; }
    }
}