using FluentAssertions;
using NUnit.Framework;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class IsOverTest {

		[TestCase(new byte[] { 0, 1, 2 }, 0)]
		[TestCase(new byte[] { 0, 1, 2 }, 1)]
		[TestCase(new byte[] { 0, 1, 2 }, 2)]
		public void ShouldNotBeOverIfPositionIsLowerThanArrayLength(byte[] array, int position) {
			var result = new ByteArrayReadStream(array, position);
			result.IsOver.Should().Be(false);
		}

		[TestCase(new byte[] { 0, 1, 2 }, 3)]
		public void ShouldOverIfPositionIsGreaterOrEqualsThanArrayLength(byte[] array, int position) {
			var result = new ByteArrayReadStream(array, position);
			result.IsOver.Should().Be(true);
		}
	}
}
