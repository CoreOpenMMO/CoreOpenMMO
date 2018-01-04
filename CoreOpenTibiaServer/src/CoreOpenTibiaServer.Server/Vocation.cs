using System;
using Newtonsoft.Json;

namespace COTS.Server {

    /// <summary>
    /// This class represents the vocation of a character.
    /// Default values represent are relative to a vocationless-character.
    /// </summary>
    public sealed class Vocation : IEquatable<Vocation> {
        public readonly uint VocationId = 0;
        public readonly uint BaseVocationId = 0;
        public readonly string VocationName = "none";
        public readonly string VocationDescription = string.Empty;

        public readonly uint TicksToGainMana = 6;
        public readonly uint ManaRegen = 1;

        public readonly uint TicksToGainHealth = 6;
        public readonly uint HealthRegen = 1;

        public readonly float MeleeAttackDamageMultiplier = 1;
        public readonly float DistanceAttackDamageMultiplier = 1;
        public readonly float DefenseMultiplier = 1;
        public readonly float ArmorMultiplier = 1;

        // What are those supposed to represent?
        // std::map<uint32_t, uint64_t> cacheMana;
        // std::map<uint32_t, uint32_t> cacheSkill[SKILL_LAST + 1];

        // What are those supposed to represent?
        // float skillMultipliers[SKILL_LAST + 1] = { 1.5f, 2.0f, 2.0f, 2.0f, 2.0f, 1.5f, 1.1f };
        // float manaMultiplier = 4.0f;

        public readonly uint CapacityGainOnLevelUp = 500;
        public readonly uint ManaGainOnLevelUp = 5;
        public readonly uint HealthPointGainOnLevelUp = 5;

        public readonly uint AttackSpeed = 1500;
        public readonly uint BaseMoveSpeed = 220;

        public readonly uint TicksToGainSoul = 120;
        public readonly uint MaximumSoul = 100;

        // What is this supposed to represent?
        // private uint8_t clientId = 0;

        // What is this supposed to represent?
        // private static uint32_t skillBase[SKILL_LAST + 1];

        [JsonConstructor]
        public Vocation(
            uint vocationId,
            uint baseVocationId,
            string vocationName,
            string vocationDescription,
            uint ticksToGainMana,
            uint manaRegen,
            uint ticksToGainHealth,
            uint healthRegen,
            uint ticksToGainSoul,
            uint maximumSoul,
            float meleeAttackDamageMultiplier,
            float distanceAttackDamageMultiplier,
            float defenseMultiplier,
            float armorMultiplier,
            uint capacityGainOnLevelUp,
            uint manaGainOnLevelUp,
            uint healthPointGainOnLevelUp,
            uint attackSpeed,
            uint baseMoveSpeed
            ) {
            if (vocationName == null)
                throw new ArgumentNullException(nameof(vocationName));
            if (vocationDescription == null)
                throw new ArgumentNullException(nameof(vocationDescription));

            VocationId = vocationId;
            BaseVocationId = baseVocationId;
            VocationName = vocationName;
            VocationDescription = vocationDescription;

            TicksToGainMana = ticksToGainMana;
            ManaRegen = manaRegen;

            TicksToGainHealth = ticksToGainHealth;
            HealthRegen = healthRegen;

            TicksToGainSoul = ticksToGainSoul;
            MaximumSoul = maximumSoul;

            MeleeAttackDamageMultiplier = meleeAttackDamageMultiplier;
            DistanceAttackDamageMultiplier = distanceAttackDamageMultiplier;
            DefenseMultiplier = defenseMultiplier;
            ArmorMultiplier = armorMultiplier;

            CapacityGainOnLevelUp = capacityGainOnLevelUp;
            ManaGainOnLevelUp = manaGainOnLevelUp;
            HealthPointGainOnLevelUp = healthPointGainOnLevelUp;

            AttackSpeed = attackSpeed;
            BaseMoveSpeed = baseMoveSpeed;
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(VocationId)
                .CombineHashCode(BaseVocationId)
                .CombineHashCode(VocationName);
        }

        public override bool Equals(object obj) => Equals(obj as Vocation);

        public bool Equals(Vocation other) {
            return other != null &&
                VocationId == other.VocationId &&
                BaseVocationId == other.BaseVocationId &&
                VocationName == other.VocationName &&
                VocationDescription == other.VocationDescription &&
                TicksToGainMana == other.TicksToGainMana &&
                ManaRegen == other.ManaRegen &&
                TicksToGainHealth == other.TicksToGainHealth &&
                HealthRegen == other.HealthRegen &&
                TicksToGainSoul == other.TicksToGainSoul &&
                MaximumSoul == other.MaximumSoul &&
                MeleeAttackDamageMultiplier == other.MeleeAttackDamageMultiplier &&
                DistanceAttackDamageMultiplier == other.DistanceAttackDamageMultiplier &&
                DefenseMultiplier == other.DefenseMultiplier &&
                ArmorMultiplier == other.ArmorMultiplier &&
                CapacityGainOnLevelUp == other.CapacityGainOnLevelUp &&
                ManaGainOnLevelUp == other.ManaGainOnLevelUp &&
                HealthPointGainOnLevelUp == other.HealthPointGainOnLevelUp &&
                AttackSpeed == other.AttackSpeed &&
                BaseMoveSpeed == other.BaseMoveSpeed;
        }

        public static Vocation DefaultKnight() {
            var knight = new Vocation(
                vocationId: 4,
                baseVocationId: 4,
                vocationName: "Knight",
                vocationDescription: "a knight",
                ticksToGainMana: 6,
                manaRegen: 2,
                ticksToGainHealth: 6,
                healthRegen: 1,
                ticksToGainSoul: 120,
                maximumSoul: 100,
                meleeAttackDamageMultiplier: 1,
                distanceAttackDamageMultiplier: 1,
                defenseMultiplier: 1,
                armorMultiplier: 1,
                capacityGainOnLevelUp: 25,
                manaGainOnLevelUp: 5,
                healthPointGainOnLevelUp: 15,
                attackSpeed: 2000,
                baseMoveSpeed: 220
                );

            return knight;
        }
    }
}