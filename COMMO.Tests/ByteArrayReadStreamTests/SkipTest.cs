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
			result.Skip((ushort)skipQuantity);
			result.Position.Should().Be(expectedPosition);
		}

		[TestCase(new byte[] { 0, 7, 3, 4 }, 0, 1)]
		public void ShouldSkipDefaultValue(byte[] array, int position, int expectedPosition) {
			var result = new ByteArrayReadStream(array, position);
			result.Skip();
			result.Position.Should().Be(expectedPosition);
		}
	}
}
