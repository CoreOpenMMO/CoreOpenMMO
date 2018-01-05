using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace COTS.Infra.CrossCutting.Network.Tests {

    [TestClass]
    public sealed class LoginRequestTests {

        [TestMethod]
        public void Equals_ActuallyEqual() {
            var first = new LoginRequest(
                clientVersion: 0,
                communicationProtocolVersion: 0,
                account: "acc",
                password: "pass");

            var second = new LoginRequest(
                clientVersion: 0,
                communicationProtocolVersion: 0,
                account: "acc",
                password: "pass"
                );

            Assert.IsTrue(first.Equals(second));
        }

        [TestMethod]
        public void EncodeAndDecode() {
            var original = new LoginRequest(
                clientVersion: 0,
                communicationProtocolVersion: 0,
                account: "acc",
                password: "pass");

            var encoded = NetworkMessage.EncodeAndPrependByteCount(original);
            var legitmateBytes = encoded
                .Skip(sizeof(int))
                .ToArray();
            var decoded = NetworkMessage.DecodeLoginRequest(legitmateBytes);

            Assert.AreEqual(
                expected: original,
                actual: decoded);
        }
    }
}