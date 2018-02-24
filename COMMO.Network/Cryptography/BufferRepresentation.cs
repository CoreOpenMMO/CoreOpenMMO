using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace COMMO.Network.Cryptography {
	[StructLayout(LayoutKind.Explicit, Pack = 8)]
	public struct BufferRepresentation {

		[FieldOffset(0)]
		public int NumberOfBytes;
		[FieldOffset(8)]
		private byte[] _byteBuffer;
		[FieldOffset(8)]
		private uint[] _uintBuffer;

		public BufferRepresentation(byte[] bufferToBoundTo) {
			if ((bufferToBoundTo.Length % 4) != 0) {
				throw new ArgumentException("The byte buffer to bound must be 4 bytes aligned");
			}
			_uintBuffer = null;
			_byteBuffer = bufferToBoundTo;
			NumberOfBytes = 0;
		}
		
/*		public static implicit operator byte[] (BufferRepresentation waveBuffer) {
			return waveBuffer.byteBuffer;
		}

		public static implicit operator int[] (BufferRepresentation waveBuffer) {
			return waveBuffer.intBuffer;
		}*/

		public byte[] ByteBuffer {
			get { return _byteBuffer; }
		}
		
		public uint[] UIntBuffer {
			get { return _uintBuffer; }
			set { }
		}
		
		public int MaxSize {
			get { return _byteBuffer.Length; }
		}

		public int ByteBufferCount {
			get { return NumberOfBytes; }
			set {
				NumberOfBytes = CheckValidityCount("ByteBufferCount", value, 1);
			}
		}

		public int UIntBufferCount {
			get { return NumberOfBytes / 4; }
			set {
				NumberOfBytes = CheckValidityCount("IntBufferCount", value, 4);
			}
		}
		
		public void Copy(Array destinationArray) {
			Array.Copy(_byteBuffer, destinationArray, NumberOfBytes);
		}

		private int CheckValidityCount(string argName, int value, int sizeOfValue) {
			var newNumberOfBytes = value * sizeOfValue;
			if ((newNumberOfBytes % 4) != 0) {
				throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count ({1}) that is not 4 bytes aligned ", argName, newNumberOfBytes));
			}

			if (value < 0 || value > (_byteBuffer.Length / sizeOfValue)) {
				throw new ArgumentOutOfRangeException(argName, String.Format("{0} cannot set a count that exceed max count {1}", argName, _byteBuffer.Length / sizeOfValue));
			}
			return newNumberOfBytes;
		}
	}
}