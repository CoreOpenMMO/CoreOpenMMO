using FluentAssertions;
using NUnit.Framework;
using System;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class SkipTest {

		[TestCase(new byte[] { 0, 7, 3, 4 }, 0, 1, 1)]
		[TestCase(new byte[] { 0, 7, 3, 4 }, 0, 2, 2)]
		[TestCase(new byte[] { 0, 7, 3, 4 }, 1, 2, 3)]
		public void ShouldSkipTheCorrectValues(byte[] array, int position, int skipQuantity, int expectedPosition) {
			var result = new ByteArrayReadStream(array, position);
			result.Skip(skipQuantity);
			result.Position.Should().Be(expectedPosition);
		}

		[TestCase(new byte[] { 0, 7, 3, 4 }, 0, 1)]
		public void ShouldSkipDefaultValue(byte[] array, int position, int expectedPosition) {
			var result = new ByteArrayReadStream(array, position);
			result.Skip();
			result.Position.Should().Be(expectedPosition);
		}

		[TestCase(new byte[] { 0, 7, 3, 4 }, 1, -2)]
		[TestCase(new byte[] { 0, 7, 3, 4 }, 1, -1)]
		[TestCase(new byte[] { 0, 7, 3, 4 }, 1, 0)]
		public void ShouldNotSkipIfValueIsNegativeOrInvalid(byte[] array, int position, int skipQuantity) {
			var objectResult = new ByteArrayReadStream(array, position);
			Action action = () => { objectResult.Skip(skipQuantity); };
			action.Should().ThrowExactly<ArgumentOutOfRangeException>();
		}

		[TestCase(new byte[] { 0, 7, 3, 4 }, 1, 4)]
		[TestCase(new byte[] { 0, 7, 3, 4 }, 1, 3)]
		[TestCase(new byte[] { 0, 7, 3, 4 }, 3, 1)]
		public void ShouldNotSkipIfValueIsGreaterOfOversTheArray(byte[] array, int position, int skipQuantity) {
			var objectResult = new ByteArrayReadStream(array, position);
			Action action = () => { objectResult.Skip(skipQuantity); };
			action.Should().ThrowExactly<ArgumentOutOfRangeException>();
		}
	}
}
