using System;

namespace COTS.GameServer.Vocations {

    public sealed class Vocation : IEquatable<Vocation> {
        public readonly uint VocationId;
        public readonly uint BaseVocationId;
        public readonly string VocationName;
        public readonly string VocationDescription;

        public readonly uint AttackSpeed;
        public readonly uint BaseMoveSpeed;

        public readonly AttributesLevelingIncrement AttributesLevelingIncrement;
        public readonly CombatMultipliers CombatMultipliers;
        public readonly SkillsLevelingDifficulty SkillsLevelingDifficulty;
        public readonly PassiveRegenerations PassiveRegenerations;

        public Vocation(
            uint vocationId,
            uint baseVocationId,
            string vocationName,
            string vocationDescription,
            uint attackSpeed,
            uint baseMoveSpeed,
            AttributesLevelingIncrement attributesLevelingIncrement,
            CombatMultipliers combatMultipliers,
            SkillsLevelingDifficulty skillsLevelingDifficulty,
            PassiveRegenerations passiveRegenerations
            ) {
            if (vocationName == null)
                throw new ArgumentNullException(nameof(vocationName));
            if (attributesLevelingIncrement == null)
                throw new ArgumentNullException(nameof(attributesLevelingIncrement));
            if (combatMultipliers == null)
                throw new ArgumentNullException(nameof(combatMultipliers));
            if (skillsLevelingDifficulty == null)
                throw new ArgumentNullException(nameof(skillsLevelingDifficulty));
            if (passiveRegenerations == null)
                throw new ArgumentNullException(nameof(passiveRegenerations));
            if (VocationDescription == null)
                throw new ArgumentNullException(nameof(VocationDescription));

            VocationId = vocationId;
            BaseVocationId = baseVocationId;
            VocationName = vocationName;
            VocationDescription = vocationDescription;
            AttributesLevelingIncrement = attributesLevelingIncrement;
            CombatMultipliers = combatMultipliers;
            SkillsLevelingDifficulty = skillsLevelingDifficulty;
            PassiveRegenerations = passiveRegenerations;
            AttackSpeed = attackSpeed;
            BaseMoveSpeed = baseMoveSpeed;
        }

        public override bool Equals(object obj) {
            return Equals(obj as Vocation);
        }

        public bool Equals(Vocation other) {
            return other != null &&
                   VocationId == other.VocationId &&
                   BaseVocationId == other.BaseVocationId &&
                   VocationName == other.VocationName &&
                   VocationDescription == other.VocationDescription &&
                   AttackSpeed == other.AttackSpeed &&
                   BaseMoveSpeed == other.BaseMoveSpeed &&
                   AttributesLevelingIncrement.Equals(other.AttributesLevelingIncrement) &&
                   CombatMultipliers.Equals(other.CombatMultipliers) &&
                   SkillsLevelingDifficulty.Equals(other.SkillsLevelingDifficulty) &&
                   PassiveRegenerations.Equals(other.PassiveRegenerations);
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(VocationId)
                .CombineHashCode(BaseVocationId)
                .CombineHashCode(VocationName)
                .CombineHashCode(VocationDescription);
        }
    }
}