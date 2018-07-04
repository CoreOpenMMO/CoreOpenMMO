using FluentAssertions;
using NUnit.Framework;
using System.Linq;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class BytesLeftToReadTest {

		[TestCase(new byte[] { 0, 1 }, 0)]
		[TestCase(new byte[] { 0, 1 }, 1)]
		[TestCase(new byte[] { 0, 1 }, 2)]
		public void ShouldReturnTheCorrectValueOfBytesToRead(byte[] array, int position) {
			var expectedValue = array.Count() - position;
			var result = new ByteArrayReadStream(array, position);
			result.BytesLeftToRead.Should().Be(expectedValue);
		}
    }
}
