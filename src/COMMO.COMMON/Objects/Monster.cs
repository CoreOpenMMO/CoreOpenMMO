namespace COMMO.Common.Objects
{
    public class Monster : Creature
    {
        public Monster(MonsterMetadata metadata)
        {
            Name = metadata.Name;

            Health = metadata.Health;

            MaxHealth = metadata.MaxHealth;

            Outfit = metadata.Outfit;

            Speed = metadata.Speed;

            Metadata = metadata;
        }

        public MonsterMetadata Metadata { get; }
    }
}