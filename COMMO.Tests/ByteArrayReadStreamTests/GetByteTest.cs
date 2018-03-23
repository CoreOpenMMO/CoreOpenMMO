using FluentAssertions;
using NUnit.Framework;
using System;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class GetByteTest {

		[TestCase(new byte[] { 0, 1 }, 0, 0)]
		[TestCase(new byte[] { 0, 1 }, 1, 1)]
		public void ShouldReturnTheCorrectValue(byte[] array, int position, byte value) {
			var result = new ByteArrayReadStream(array, position);
			result.GetByte().Should().Be(value);
		}

		[TestCase(new byte[] { 0, 1 }, 2)]
		public void ShouldThrowAnExceptionIfTryingToGetAnValueOfAnEndedByteArray(byte[] array, int position) {
			var resultObject = new ByteArrayReadStream(array, position);
			Action action = () => { resultObject.GetByte(); };
			action.Should().ThrowExactly<IndexOutOfRangeException>();
		}
	}
}
