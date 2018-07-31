namespace COMMO.OTB.Tests {
	using NUnit.Framework;
	using System.Collections.Generic;

	public sealed class OTBParsingStreamTests {

		[Test]
		[TestCase(
			new byte[] { },
			new byte[] { })]
		[TestCase(
			new byte[] { 0 },
			new byte[] { 0 })]
		[TestCase(
			new byte[] { 1, 2, 0 },
			new byte[] { 1, 2, 0 })]
		public void ReadByte_WithoutEscapeBytes(byte[] buffer, byte[] expectedBytes) {
			var stream = new OTBParsingStream(buffer);

			var bytesRead = new List<byte>();
			while (!stream.IsOver)
				bytesRead.Add(stream.ReadByte());

			var actualBytes = bytesRead.ToArray();

			Assert.AreEqual(
				expected: expectedBytes,
				actual: actualBytes);
		}

		[Test]
		[TestCase(
			new byte[] { },
			new byte[] { })]
		[TestCase(
			new byte[] { 0 },
			new byte[] { 0 })]
		[TestCase(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, (byte)OTBMarkupByte.Escape },
			new byte[] { 1, (byte)OTBMarkupByte.Escape })]
		[TestCase(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, (byte)OTBMarkupByte.Start },
			new byte[] { 1, (byte)OTBMarkupByte.Start })]
		[TestCase(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, (byte)OTBMarkupByte.End },
			new byte[] { 1, (byte)OTBMarkupByte.End })]
		[TestCase(
			new byte[] { 1, (byte)OTBMarkupByte.Escape, 3 },
			new byte[] { 1, 3 })]
		[TestCase(
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

			Assert.AreEqual(
				expected: expectedBytes,
				actual: actualBytes);
		}
	}
}
