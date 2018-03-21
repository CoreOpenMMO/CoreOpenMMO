using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace COMMO.Tests.ByteArrayReadStreamTests {
	public class ReadUInt16Test {

		[TestCase(new byte[] { 0, 7 }, 2)]
		public void ShouldThrowExceptionIfArrayIsOver(byte[] array, int position) {
			var objectResult = new ByteArrayReadStream(array, position);
			Action action = () => { objectResult.ReadUInt16(); };
			action.Should().ThrowExactly<InvalidOperationException>();
		}

		[TestCase((ushort)7)]
		[TestCase((ushort)1212)]
		[TestCase(ushort.MaxValue)]
		[TestCase(ushort.MinValue)]
		public void ShouldReadAndSkipPositionsProperlySingleValue(ushort value) {
			var bytearray = BitConverter.GetBytes(value);
			var byteArrayReader = new ByteArrayReadStream(bytearray);
			var result = byteArrayReader.ReadUInt16();

			result.Should().Be(value);
			byteArrayReader.Position.Should().Be(sizeof(ushort));
			byteArrayReader.IsOver.Should().BeTrue();
		}
		[TestCase(new ushort[] { 0, 7 }, false)]
		[TestCase(new ushort[] { 654, 9687, 212 }, false)]
		[TestCase(new ushort[] { ushort.MaxValue, ushort.MinValue, 3 }, false)]
		[TestCase(new ushort[] { 2 }, true)]
		public void ShouldReadAndSkipPositionsProperlyArrayOfValues(ushort[] values, bool isOver) {
			var bytearray = values.SelectMany(BitConverter.GetBytes).ToArray();
			var byteArrayReader = new ByteArrayReadStream(bytearray);
			var result = byteArrayReader.ReadUInt16();

			result.Should().Be(values[0]);
			byteArrayReader.Position.Should().Be(sizeof(ushort));
			byteArrayReader.IsOver.Should().Be(isOver);
		}
	}
}
