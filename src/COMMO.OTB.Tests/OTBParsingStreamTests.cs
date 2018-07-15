namespace COMMO.OTB.Tests {
	using System.Collections.Generic;
	using Xunit;

	public sealed class OTBParsingStreamTests {

		[Theory]
		[InlineData(
			new byte[] { },
			new byte[] { })]
		[InlineData(
			new byte[] { 0 },
			new byte[] { 0 })]
		[InlineData(
			new byte[] { 1, 2, 0 },
			new byte[] { 1, 2, 0 })]
		public void ReadByte_WithoutEscapeBytes(byte[] buffer, byte[] expectedBytes) {
			var stream = new OTBParsingStream(buffer);

			var bytesRead = new List<byte>();
			while (!stream.IsOver)
				bytesRead.Add(stream.ReadByte());

			var actualBytes = bytesRead.ToArray();

			Assert.Equal(
				expected: expectedBytes,
				actual: actualBytes);
		}

		[Theory]
		[InlineData(
			new byte[] { },
			new byte[] { })]
		[InlineData(
			new byte[] { 0 },
			new byte[] { 0 })]
		[InlineData(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, (byte)OTBMarkupByte.Escape },
			new byte[] { 1, (byte)OTBMarkupByte.Escape })]
		[InlineData(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, (byte)OTBMarkupByte.Start },
			new byte[] { 1, (byte)OTBMarkupByte.Start })]
		[InlineData(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, (byte)OTBMarkupByte.End },
			new byte[] { 1, (byte)OTBMarkupByte.End })]
		[InlineData(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, 3 },
			new byte[] { 1, 3 })]
		[InlineData(
			new byte[] { (byte)OTBMarkupByte.Start, 3, (byte)OTBMarkupByte.End },
			new byte[] { (byte)OTBMarkupByte.Start, 3, (byte)OTBMarkupByte.End })]
		public void ReadByte_WithEscapeBytes(byte[] buffer, byte[] expectedBytes) {
			var stream = new OTBParsingStream(buffer);

			var bytesRead = new List<byte>();
			while (!stream.IsOver) {
				var value = stream.ReadByte();
				bytesRead.Add(value);
			}

			var actualBytes = bytesRead.ToArray();

			Assert.Equal(
				expected: expectedBytes,
				actual: actualBytes);
		}
	}
}
