using FluentAssertions;
using NUnit.Framework;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class ReadByteTest
    {

		[TestCase(new byte[] { 0, 7, 3 }, 0, 0, false)]
		[TestCase(new byte[] { 0, 7, 3 }, 1, 7, false)]
		[TestCase(new byte[] { 0, 7, 3 }, 2, 3, true)]
		public void ShouldReturnTheCorrectValue(byte[] array, int position, byte value, bool shouldOver) {
			var result = new ByteArrayReadStream(array, position);
			result.Position.Should().Be(position);
			result.ReadByte().Should().Be(value);
			result.Position.Should().Be(position + 1);
			result.IsOver.Should().Be(shouldOver);
		}
	}
}
