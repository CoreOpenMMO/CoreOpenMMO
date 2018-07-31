namespace COMMO.Server.Tests {
	using COMMO.Server.World;
	using NUnit.Framework;
	using System;

	[TestFixture]
	public sealed class CoordinateTests {

		[Test, Combinatorial]
		public void Translate_WithTwoOffsets(
			[Values(-1, 0, 1)] int xOffset,
			[Values(-1, 0, 1)] int yOffset
			) {
			var startCoord = new Coordinate(
				x: 0,
				y: 0,
				z: 0);

			var translatedCoord = startCoord.Translate(
				xOffset: xOffset,
				yOffset: yOffset);

			var expectedX = startCoord.X + xOffset;
			var expectedY = startCoord.Y + yOffset;
			var expectedZ = startCoord.Z;

			Assert.AreEqual(
				expected: expectedX,
				actual: translatedCoord.X);

			Assert.AreEqual(
				expected: expectedY,
				actual: translatedCoord.Y);

			Assert.AreEqual(
				expected: expectedZ,
				actual: translatedCoord.Z);
		}

		[Test, Combinatorial]
		public void Translate_WithThreeOffsets(
			[Values(-1, 0, 1)] int xOffset,
			[Values(-1, 0, 1)] int yOffset,
			[Values(-1, 0, 1)] sbyte zOffset
			) {
			var startCoord = new Coordinate(
				x: 0,
				y: 0,
				z: 0);

			var translatedCoord = startCoord.Translate(
				xOffset: xOffset,
				yOffset: yOffset,
				zOffset: zOffset);

			var expectedX = startCoord.X + xOffset;
			var expectedY = startCoord.Y + yOffset;
			var expectedZ = startCoord.Z + zOffset;

			Assert.AreEqual(
				expected: expectedX,
				actual: translatedCoord.X);

			Assert.AreEqual(
				expected: expectedY,
				actual: translatedCoord.Y);

			Assert.AreEqual(
				expected: expectedZ,
				actual: translatedCoord.Z);
		}

		[Test, Combinatorial]
		public void FromTFSCoordinates(
			[Values((UInt16)0, (UInt16)100)] UInt16 x,
			[Values((UInt16)0, (UInt16)100)] UInt16 y,
			[Values((byte)0, (byte)15)] byte z
			) {

			var coordinate = Coordinate.FromTFSCoordinates(
				x: x,
				y: y,
				z: z);

			int expectedX = x;
			int expectedY = -y;
			
			// In TFS / OTBM, the 'lowest' Z coordinate is 15.
			// 7 is the ground floor
			// and 0 is the 'highest' Z coordinate.
			sbyte expectedZ = (sbyte)-((sbyte)z - Coordinate.TFSGroundFloorZCoordinate);

			Assert.AreEqual(
				expected: expectedX,
				actual: coordinate.X);

			Assert.AreEqual(
				expected: expectedY,
				actual: coordinate.Y);

			Assert.AreEqual(
				expected: expectedZ,
				actual: coordinate.Z);
		}
	}
}
