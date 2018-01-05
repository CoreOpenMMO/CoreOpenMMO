using System;

namespace COTS.GameServer.Vocations {

    public sealed class CombatMultipliers : IEquatable<CombatMultipliers> {
        public readonly float MeleeAttack;
        public readonly float DistanceAttack;
        public readonly float Defense;
        public readonly float Armor;

        public CombatMultipliers(
            float meleeAttack,
            float distanceAttack,
            float defense,
            float armor
            ) {
            if (meleeAttack.IsNanOrInfinity() || meleeAttack <= 0)
                throw new ArgumentOutOfRangeException(nameof(meleeAttack) + " must be finite and greater than zero.");
            if (distanceAttack.IsNanOrInfinity() || distanceAttack <= 0)
                throw new ArgumentOutOfRangeException(nameof(distanceAttack) + " must be finite and greater than zero.");
            if (defense.IsNanOrInfinity() || defense <= 0)
                throw new ArgumentOutOfRangeException(nameof(defense) + " must be finite and greater than zero.");
            if (armor.IsNanOrInfinity() || armor <= 0)
                throw new ArgumentOutOfRangeException(nameof(armor) + " must be finite and greater than zero.");

            MeleeAttack = meleeAttack;
            DistanceAttack = distanceAttack;
            Defense = defense;
            Armor = armor;
        }

        public override bool Equals(object obj) {
            return Equals(obj as CombatMultipliers);
        }

        public bool Equals(CombatMultipliers other) {
            return other != null &&
                   MeleeAttack == other.MeleeAttack &&
                   DistanceAttack == other.DistanceAttack &&
                   Defense == other.Defense &&
                   Armor == other.Armor;
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(MeleeAttack)
                .CombineHashCode(DistanceAttack)
                .CombineHashCode(Defense)
                .CombineHashCode(Armor);
        }
    }
}