using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace COTS.Infra.CrossCutting.Network.Tests {

    [TestClass]
    public sealed class LoginResponseTests {

        [TestMethod]
        public void Equals_ActuallyEqual() {
            var first = new LoginResponse(
                status: LoginResponse.ResponseType.AccountBanned,
                characterNames: new string[] { "toon1", "toon2" },
                premiumAccountDaysRemaining: 3);

            var second = new LoginResponse(
                status: LoginResponse.ResponseType.AccountBanned,
                characterNames: new string[] { "toon1", "toon2" },
                premiumAccountDaysRemaining: 3);

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void EncodeAndDecode() {
            var original = new LoginResponse(
                status: LoginResponse.ResponseType.AccountBanned,
                characterNames: new string[] { "toon1", "toon2" },
                premiumAccountDaysRemaining: 3);

            var encoded = NetworkMessage.EncodeAndPrependByteCount(original);
            var legitmateBytes = encoded
                .Skip(sizeof(int))
                .ToArray();
            var decoded = NetworkMessage.DecodeLoginResponse(legitmateBytes);

            Assert.AreEqual(
                expected: original, 
                actual: decoded);
        }
    }
}