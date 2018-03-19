using FluentAssertions;
using NUnit.Framework;
using System;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class ConstructorTest {

		[Test]
		public void ShouldNotCreateObjectIfArrayIsNull() {
			Action action = () => { new ByteArrayReadStream(null as byte[], 0); };
			action.Should().ThrowExactly<ArgumentNullException>();
		}

		[Test]
		public void ShouldNotCreateObjectIfPositionIsNegative() {
			Action action = () => { new ByteArrayReadStream(new byte[] { 0, 1 }, -1); };
			action.Should().ThrowExactly<ArgumentOutOfRangeException>();
		}

		[Test]
		public void ShouldNotCreateObjectIfPositionIsGreaterThanArrayLength() {
			Action action = () => { new ByteArrayReadStream(new byte[] { 0, 1 }, 3); };
			action.Should().ThrowExactly<ArgumentOutOfRangeException>();
		}

		[TestCase(new byte[] { 0, 1 }, 0)]
		[TestCase(new byte[] { 0, 1 }, 1)]
		[TestCase(new byte[] { 0, 1 }, 2)]
		public void ShouldCreateIfArgumentsAreValid(byte[] array, int position) => 
			new ByteArrayReadStream(array, position);
	}
}
