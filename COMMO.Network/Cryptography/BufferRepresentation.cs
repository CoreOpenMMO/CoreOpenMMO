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
		
		public void Copy(Array destinationArray) {
			Array.Copy(_byteBuffer, destinationArray, NumberOfBytes);
		}
	}
}