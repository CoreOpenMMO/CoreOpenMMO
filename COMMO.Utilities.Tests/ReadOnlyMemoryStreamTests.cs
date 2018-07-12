namespace COMMO.Utilities.Tests {
	using COMMO.Utilities;
	using Xunit;
	using System;

	public sealed class ReadOnlyMemoryStreamTests {

		[Theory]
		[InlineData(0, 0, 0)]
		[InlineData(1, 0, 1)]
		[InlineData(1, 1, 0)]
		[InlineData(2, 0, 2)]
		[InlineData(2, 2, 0)]
		public void BytesLeftToRead_ReturningCorrectValueAfter_Constructor(
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
		public void BytesLeftToRead_ReturningCorrectValueAfter_ReadByte() {
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
		[InlineData((byte)2)]
		[InlineData((byte)3, (byte)17)]
		public void ReadByte_ReturnsCorrectValues(params byte[] bufferData) {
			// Copying to make sure that if ReadByte screws the underlaying array, I
			// still have the original values
			var buffer = new byte[bufferData.Length];
			Array.Copy(sourceArray: bufferData, destinationArray: buffer, length: bufferData.Length);

			var stream = new ReadOnlyMemoryStream(buffer);

			for (int i = 0; i < buffer.Length; i++) {
				Assert.Equal(expected: bufferData[i],
					actual: stream.ReadByte());
			}
		}
	}
}
