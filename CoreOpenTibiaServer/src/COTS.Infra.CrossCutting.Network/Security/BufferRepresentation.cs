using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace COTS.Infra.CrossCutting.Network.Security {
	[StructLayout(LayoutKind.Explicit, Pack = 2)]
	public class BufferRepresentation {

		[FieldOffset(0)]
		public int NumberOfBytes;
		[FieldOffset(8)]
		private byte[] byteBuffer;
		[FieldOffset(8)]
		private uint[] intBuffer;

		public BufferRepresentation(byte[] bufferToBoundTo) {
			if ((bufferToBoundTo.Length % 4) != 0) {
				throw new ArgumentException("The byte buffer to bound must be 4 bytes aligned");
			}
			byteBuffer = bufferToBoundTo;
			NumberOfBytes = 0;
		}
		
/*		public static implicit operator byte[] (BufferRepresentation waveBuffer) {
			return waveBuffer.byteBuffer;
		}

		public static implicit operator int[] (BufferRepresentation waveBuffer) {
			return waveBuffer.intBuffer;
		}*/

		public byte[] ByteBuffer {
			get { return byteBuffer; }
		}
		
		public uint[] IntBuffer {
			get { return intBuffer; }
			set { }
		}
		
		public int MaxSize {
			get { return byteBuffer.Length; }
		}

		public int ByteBufferCount {
			get { return NumberOfBytes; }
			set {
				NumberOfBytes = CheckValidityCount("ByteBufferCount", value, 1);
			}
		}

		public int IntBufferCount {
			get { return NumberOfBytes / 4; }
			set {
				NumberOfBytes = CheckValidityCount("IntBufferCount", value, 4);
			}
		}
		
		public void Copy(Array destinationArray) {
			Array.Copy(byteBuffer, destinationArray, NumberOfBytes);
		}

		private int CheckValidityCount(string argName, int value, int sizeOfValue) {
			var newNumberOfBytes = value * sizeOfValue;
			if ((newNumberOfBytes % 4) != 0) {
				throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count ({1}) that is not 4 bytes aligned ", argName, newNumberOfBytes));
			}

			if (value < 0 || value > (byteBuffer.Length / sizeOfValue)) {
				throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count that exceed max count {1}", argName, byteBuffer.Length / sizeOfValue));
			}
			return newNumberOfBytes;
		}
	}
}