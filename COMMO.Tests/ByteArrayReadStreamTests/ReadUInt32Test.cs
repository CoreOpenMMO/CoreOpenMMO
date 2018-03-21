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

		[TestCase((uint)7)]
		[TestCase((uint)1212)]
		[TestCase(uint.MaxValue)]
		[TestCase(uint.MinValue)]
		public void ShouldReadAndSkipPositionsProperly(uint value) {
			var bytearray = BitConverter.GetBytes(value);
			var result = new ByteArrayReadStream(bytearray)
				.ReadUInt32();

			result.Should().Be(value);
		}
	}
}
