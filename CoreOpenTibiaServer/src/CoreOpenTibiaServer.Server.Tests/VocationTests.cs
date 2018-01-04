using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace COTS.Server.Tests {

    [TestClass]
    public sealed class VocationTests {

        [TestMethod]
        public void Equals_ActuallyEqual() {
            var first = new Vocation(
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

            var second = new Vocation(
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

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void SerializationAndDeserialization() {
            var knight = Vocation.DefaultKnight();
            var serialized = SerializationManager.Serialize(knight);
            var deserialized = SerializationManager.DeserializeVocation(serialized);

            Assert.AreEqual(
                expected: knight,
                actual: deserialized);
        }
    }
}