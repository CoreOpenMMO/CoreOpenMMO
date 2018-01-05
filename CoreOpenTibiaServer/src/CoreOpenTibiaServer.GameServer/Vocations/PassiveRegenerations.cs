using System;

namespace COTS.GameServer.Vocations {

    public sealed class PassiveRegenerations : IEquatable<PassiveRegenerations> {
        public readonly ushort TicksToRegenerateHealth;
        public readonly ushort HealthRegeneration;

        public readonly ushort TicksToRegenerateMana;
        public readonly ushort ManaRegeneration;

        public readonly ushort TicksToRegenerateSoul;
        public readonly ushort SoulRegeneration;

        public PassiveRegenerations(
            ushort ticksToRegenerateHealth,
            ushort healthRegeneration,
            ushort ticksToRegenerateMana,
            ushort manaRegeneration,
            ushort ticksToRegenerateSoul,
            ushort soulRegeneration
            ) {
            TicksToRegenerateHealth = ticksToRegenerateHealth;
            HealthRegeneration = healthRegeneration;
            TicksToRegenerateMana = ticksToRegenerateMana;
            ManaRegeneration = manaRegeneration;
            TicksToRegenerateSoul = ticksToRegenerateSoul;
            SoulRegeneration = soulRegeneration;
        }

        public override bool Equals(object obj) {
            return Equals(obj as PassiveRegenerations);
        }

        public bool Equals(PassiveRegenerations other) {
            return other != null &&
                   TicksToRegenerateHealth == other.TicksToRegenerateHealth &&
                   HealthRegeneration == other.HealthRegeneration &&
                   TicksToRegenerateMana == other.TicksToRegenerateMana &&
                   ManaRegeneration == other.ManaRegeneration &&
                   TicksToRegenerateSoul == other.TicksToRegenerateSoul &&
                   SoulRegeneration == other.SoulRegeneration;
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(TicksToRegenerateHealth)
                .CombineHashCode(HealthRegeneration)
                .CombineHashCode(TicksToRegenerateMana)
                .CombineHashCode(ManaRegeneration);
        }
    }
}