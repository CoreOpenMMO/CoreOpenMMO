using COMMO.Common.Structures;
using System.Xml.Linq;

namespace COMMO.FileFormats.Xml.Spawns
{
    public class Monster
    {
        public static Monster Load(Spawn spawn, XElement monsterNode)
        {
            var monster = new Monster();

            monster.Name = (string)monsterNode.Attribute("name");

            monster.Position = new Position(spawn.Center.X + (int)monsterNode.Attribute("x"), spawn.Center.Y + (int)monsterNode.Attribute("y"), spawn.Center.Z);

            monster.SpawnTime = (uint)monsterNode.Attribute("spawntime");

            return monster;
        }

        public string Name { get; set; }

        public Position Position { get; set; }

        public uint SpawnTime { get; set; }
    }
}