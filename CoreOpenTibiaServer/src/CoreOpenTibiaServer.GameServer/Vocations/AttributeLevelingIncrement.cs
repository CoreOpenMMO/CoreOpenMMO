using System;

namespace COTS.GameServer.Vocations {

    /// <summary>
    /// When a character level up, his attributes maximum values are incremented
    /// depending on his vocation.
    /// This class stores such increments.
    /// </summary>
    public sealed class AttributesLevelingIncrement : IEquatable<AttributesLevelingIncrement> {
        public readonly ushort HealthPointsPerLevel;
        public readonly ushort ManaPointsPerLevel;
        public readonly ushort CapacityPointsPerLevel;

        public AttributesLevelingIncrement(
            ushort healthPointsPerLevel,
            ushort manaPointsPerLevel,
            ushort capacityPointsPerLevel
            ) {
            HealthPointsPerLevel = healthPointsPerLevel;
            ManaPointsPerLevel = manaPointsPerLevel;
            CapacityPointsPerLevel = capacityPointsPerLevel;
        }

        public override bool Equals(object obj) {
            return Equals(obj as AttributesLevelingIncrement);
        }

        public bool Equals(AttributesLevelingIncrement other) {
            return other != null &&
                   HealthPointsPerLevel == other.HealthPointsPerLevel &&
                   ManaPointsPerLevel == other.ManaPointsPerLevel &&
                   CapacityPointsPerLevel == other.CapacityPointsPerLevel;
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(HealthPointsPerLevel)
                .CombineHashCode(ManaPointsPerLevel)
                .CombineHashCode(CapacityPointsPerLevel);
        }
    }
}