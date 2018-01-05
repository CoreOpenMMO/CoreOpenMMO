using System;

namespace COTS.GameServer.Vocations {

    /// <summary>
    /// For a skill to level up the character must use it.
    /// For instance, to level up the "Axe" skill, it's necessary
    /// to melee-attack an enemy while using an weapon of time "Axe".
    /// Each time a player's skill level up, it gets harder to level up again.
    /// The number of 'uses' required to level up follow a geometric progression.
    /// This class stores the the 'ratio' of the geometric progressions.
    /// </summary>
    public sealed class SkillsLevelingDifficulty : IEquatable<SkillsLevelingDifficulty> {
        public readonly float Axe;
        public readonly float Sword;
        public readonly float Club;
        public readonly float Distance;
        public readonly float Fist;
        public readonly float Shielding;
        public readonly float Fishing;
        public readonly float Magic;

        public SkillsLevelingDifficulty(
            float axe,
            float sword,
            float club,
            float distance,
            float fist,
            float shielding,
            float fishing,
            float magic
            ) {
            if (axe.IsNanOrInfinity() || axe < 1)
                throw new ArgumentOutOfRangeException(nameof(axe) + " must be finite and equal to or greater than 1.");
            if (sword.IsNanOrInfinity() || sword < 1)
                throw new ArgumentOutOfRangeException(nameof(sword) + " must be finite and equal to or greater than 1.");
            if (club.IsNanOrInfinity() || club < 1)
                throw new ArgumentOutOfRangeException(nameof(club) + " must be finite and equal to or greater than 1.");
            if (distance.IsNanOrInfinity() || distance < 1)
                throw new ArgumentOutOfRangeException(nameof(distance) + " must be finite and equal to or greater than 1.");
            if (fist.IsNanOrInfinity() || fist < 1)
                throw new ArgumentOutOfRangeException(nameof(fist) + " must be finite and equal to or greater than 1.");
            if (shielding.IsNanOrInfinity() || shielding < 1)
                throw new ArgumentOutOfRangeException(nameof(shielding) + " must be finite and equal to or greater than 1.");
            if (fishing.IsNanOrInfinity() || fishing < 1)
                throw new ArgumentOutOfRangeException(nameof(fishing) + " must be finite and equal to or greater than 1.");
            if (magic.IsNanOrInfinity() || magic < 1)
                throw new ArgumentOutOfRangeException(nameof(magic) + " must be finite and equal to or greater than 1.");

            Axe = axe;
            Sword = sword;
            Club = club;
            Distance = distance;
            Fist = fist;
            Shielding = shielding;
            Fishing = fishing;
            Magic = magic;
        }

        public override bool Equals(object obj) {
            return Equals(obj as SkillsLevelingDifficulty);
        }

        public bool Equals(SkillsLevelingDifficulty other) {
            return other != null &&
                   Axe == other.Axe &&
                   Sword == other.Sword &&
                   Club == other.Club &&
                   Distance == other.Distance &&
                   Fist == other.Fist &&
                   Shielding == other.Shielding &&
                   Fishing == other.Fishing &&
                   Magic == other.Magic;
        }

        public override int GetHashCode() {
            return HashHelper.Start
                .CombineHashCode(Axe)
                .CombineHashCode(Fist)
                .CombineHashCode(Distance)
                .CombineHashCode(Shielding)
                .CombineHashCode(Magic);
        }
    }
}