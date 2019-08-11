using System.Xml.Linq;
using System.Collections.Generic;

namespace COMMO.FileFormats.Xml.Spawns
{
    public class SpawnFile
    {
        public static SpawnFile Load(string path)
        {
            var file = new SpawnFile();

            file.Spawns = new List<Spawn>();

            foreach (var spawnNode in XElement.Load(path).Elements("spawn") )
            {
                file.Spawns.Add( Spawn.Load(spawnNode) );
            }

            return file;
        }

		public List<Spawn> Spawns { get; private set; }
    }
}