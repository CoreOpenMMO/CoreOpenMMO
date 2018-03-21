using FluentAssertions;
using NUnit.Framework;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class ReadByteTest {

		[TestCase(new byte[] { 0, 7, 3 }, 0, 0)]
		[TestCase(new byte[] { 0, 7, 3 }, 1, 7)]
		[TestCase(new byte[] { 0, 7, 3 }, 2, 3)]
		public void ShouldReturnTheCorrectValue(byte[] array, int position, byte value) {
			var result = new ByteArrayReadStream(array, position);
			result.Position.Should().Be(position);
			result.ReadByte().Should().Be(value);
			result.Position.Should().Be(position + 1);
		}
	}
}
