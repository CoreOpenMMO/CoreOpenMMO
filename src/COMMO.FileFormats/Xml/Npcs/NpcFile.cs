using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace COMMO.FileFormats.Xml.Npcs
{
    public class NpcFile
    {
        public static NpcFile Load(string path)
        {
            var file = new NpcFile();

            file.Npcs = new List<Npc>();

            if (Directory.Exists(path) )
            {
                foreach (var fileName in Directory.GetFiles(path) )
                {
                    file.Npcs.Add( Npc.Load( XElement.Load(fileName) ) );
                }
            }

            return file;
        }

        public List<Npc> Npcs { get; private set; }
    }
}