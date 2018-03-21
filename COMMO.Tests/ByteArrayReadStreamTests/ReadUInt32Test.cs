using FluentAssertions;
using NUnit.Framework;
using System;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class ReadUInt32Test {

		[TestCase(new byte[] { 0, 7 }, 2)]
		public void ShouldThrowExceptionIfArrayIsOver(byte[] array, int position) {
			var objectResult = new ByteArrayReadStream(array, position);
			Action action = () => { objectResult.ReadUInt32(); };
			action.Should().ThrowExactly<InvalidOperationException>();
		}

		[TestCase(new byte[] { 1, 7, 3, 4 }, 0, 1, 1)]
		[TestCase(new byte[] { 1, 7, 3, 4 }, 1, 2, 7)]
		public void ShouldReadAndSkipPositionsProperly(byte[] array, int position, int expectedPosition, int expectedResult) {
			var objectResult = new ByteArrayReadStream(array, position);
			var result = objectResult.ReadUInt32();
			result.Should().Be((uint)expectedResult);
			objectResult.Position.Should().Be(expectedPosition);
		}
	}
}
