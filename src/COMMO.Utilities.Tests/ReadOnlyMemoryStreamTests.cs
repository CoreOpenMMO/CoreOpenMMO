namespace COMMO.Utilities.Tests {
	using COMMO.Utilities;
	using Xunit;
	using System;
	using System.Collections.Generic;

	public sealed class ReadOnlyMemoryStreamTests {

		[Theory]
		[InlineData(0, 0, 0)]
		[InlineData(1, 0, 1)]
		[InlineData(1, 1, 0)]
		[InlineData(2, 0, 2)]
		[InlineData(2, 2, 0)]
		public void BytesLeftToRead_ReturnsCorrectValueAfter_Constructor(
			int bufferLength,
			int startPosition,
			int bytesLeft
			) {
			var stream = new ReadOnlyMemoryStream(
				buffer: new byte[bufferLength],
				position: startPosition);

			Assert.Equal(expected: bytesLeft,
				actual: stream.BytesLeftToRead);
		}

		[Fact]
		public void BytesLeftToRead_ReturnsCorrectValueAfter_ReadByte() {
			var stream = new ReadOnlyMemoryStream(buffer: new byte[10]);

			var oldLeft = stream.BytesLeftToRead;
			stream.ReadByte();
			var newLeft = stream.BytesLeftToRead;

			Assert.True(oldLeft - newLeft == 1);
		}

		[Theory]
		[InlineData(10, 5, 5)]
		public void ReadByte_Throws_AfterReadingEntireBuffer(int bufferLength, int startPosition, int bytesToRead) {
			// Working around the ref struct constraints
			var reachedThrowLine = false;

			Assert.ThrowsAny<InvalidOperationException>(() => {
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

		[Theory]
		[InlineData(
			new byte[] { },
			new byte[] { })]
		[InlineData(
			new byte[] { 0 },
			new byte[] { 0 })]
		[InlineData(
			new byte[] { 1 },
			new byte[] { 1 })]
		[InlineData(
			new byte[] { 1, 3 },
			new byte[] { 1, 3 })]
		public void ReadByte_ReturnsCorrectValues(byte[] buffer, byte[] expectedBytes) {
			var stream = new ReadOnlyMemoryStream(buffer);

			var bytesRead = new List<byte>();
			while (!stream.IsOver)
				bytesRead.Add(stream.ReadByte());

			var actualBytes = bytesRead.ToArray();

			Assert.Equal(
				expected: expectedBytes,
				actual: actualBytes);
		}
	}
}
