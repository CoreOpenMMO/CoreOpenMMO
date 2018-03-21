using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

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
		public void ShouldReadAndSkipPositionsProperlySingleValue(uint value) {
			var bytearray = BitConverter.GetBytes(value);
			var byteArrayReader = new ByteArrayReadStream(bytearray);
			var result = byteArrayReader.ReadUInt32();

			result.Should().Be(value);
			byteArrayReader.Position.Should().Be(sizeof(UInt32));
			byteArrayReader.IsOver.Should().BeTrue();
		}
		[TestCase(new uint[] { 0, 7 }, false)]
		[TestCase(new uint[] { 654, 9687, 212 }, false)]
		[TestCase(new uint[] { uint.MaxValue, uint.MinValue, 3 }, false)]
		[TestCase(new uint[] { 2 }, true)]
		public void ShouldReadAndSkipPositionsProperlyArrayOfValues(uint[] values, bool isOver) {
			var bytearray = values.SelectMany(BitConverter.GetBytes).ToArray();
			var byteArrayReader = new ByteArrayReadStream(bytearray);
			var result = byteArrayReader.ReadUInt32();

			result.Should().Be(values[0]);
			byteArrayReader.Position.Should().Be(sizeof(UInt32));
			byteArrayReader.IsOver.Should().Be(isOver);
		}
	}
}
