namespace COMMO.Utilities.Tests {
	using COMMO.Utilities;
	using NUnit.Framework;
	using System;
	using System.Collections.Generic;

	[TestFixture]
	public sealed class ReadOnlyMemoryStreamTests {

		[Test, Sequential]
		public void BytesLeftToRead_ReturnsCorrectValueAfter_Constructor(
			[Values(0, 1, 1, 2, 2)] int bufferLength,
			[Values(0, 0, 1, 0, 2)]int startPosition,
			[Values(0, 1, 0, 2, 0)]int bytesLeft
			) {
			var stream = new ReadOnlyMemoryStream(
				buffer: new byte[bufferLength],
				position: startPosition);

			Assert.AreEqual(
				expected: bytesLeft,
				actual: stream.BytesLeftToRead);
		}

		[Test]
		public void BytesLeftToRead_ReturnsCorrectValueAfter_ReadByte() {
			var stream = new ReadOnlyMemoryStream(buffer: new byte[10]);

			var oldLeft = stream.BytesLeftToRead;
			stream.ReadByte();
			var newLeft = stream.BytesLeftToRead;

			Assert.True(oldLeft - newLeft == 1);
		}

		[Test, Sequential]
		public void ReadByte_Throws_AfterReadingEntireBuffer(
			[Values(10)] int bufferLength,
			[Values(5)] int startPosition,
			[Values(5)] int bytesToRead
			) {
			// Working around the ref struct constraints
			var reachedThrowLine = false;

			Assert.Throws<InvalidOperationException>(() => {
				var stream = new ReadOnlyMemoryStream(
					buffer: new byte[bufferLength],
					position: startPosition);

				for (int i = 0; i < bytesToRead; i++)
					stream.ReadByte();

				reachedThrowLine = true;
				stream.ReadByte();
			});


			Assert.True(reachedThrowLine);
		}

		[Test, Sequential]
		[TestCase(
			new byte[] { },
			new byte[] { })]
		[TestCase(
			new byte[] { 0 },
			new byte[] { 0 })]
		[TestCase(
			new byte[] { 1 },
			new byte[] { 1 })]
		[TestCase(
			new byte[] { 1, 3 },
			new byte[] { 1, 3 })]
		public void ReadByte_ReturnsCorrectValues(byte[] buffer, byte[] expectedBytes) {
			var stream = new ReadOnlyMemoryStream(buffer);

			var bytesRead = new List<byte>();
			while (!stream.IsOver)
				bytesRead.Add(stream.ReadByte());

			var actualBytes = bytesRead.ToArray();

			Assert.AreEqual(
				expected: expectedBytes,
				actual: actualBytes);
		}
	}
}
