using System.Xml.Linq;
using System.Collections.Generic;
using COMMO.Common.Structures;

namespace COMMO.FileFormats.Xml.Spawns
{
    public class Spawn
    {
        public static Spawn Load(XElement spawnNode)
        {
            var spawn = new Spawn();

            spawn.Center = new Position( (int)spawnNode.Attribute("centerx"), (int)spawnNode.Attribute("centery"), (int)spawnNode.Attribute("centerz") );

            spawn.Radius = (uint)spawnNode.Attribute("radius");

            spawn.Monsters = new List<Monster>();

            foreach (var monsterNode in spawnNode.Elements("monster") )
            {
                spawn.Monsters.Add( Monster.Load(spawn, monsterNode) );
            }

            spawn.Npcs = new List<Npc>();

            foreach (var npcNode in spawnNode.Elements("npc") )
            {
                spawn.Npcs.Add( Npc.Load(spawn, npcNode) );
            }

            return spawn;
        }

        public Position Center { get; set; }

        public uint Radius { get; set; }

        public List<Monster> Monsters { get; private set; }

        public List<Npc> Npcs { get; private set; }
    }
}