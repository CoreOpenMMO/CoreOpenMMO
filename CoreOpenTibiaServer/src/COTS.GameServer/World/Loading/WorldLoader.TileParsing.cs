using COTS.GameServer.OTBParsing;
using System;

namespace COTS.GameServer.World.Loading {

	public static partial class WorldLoader {

		public static void ParseTileAreaNode(ParsingTree parsingTree, ParsingNode tileAreaNode) {
			if (parsingTree == null)
				throw new ArgumentNullException(nameof(parsingTree));
			if (tileAreaNode == null)
				throw new ArgumentNullException(nameof(tileAreaNode));

			var stream = new ParsingStream(
				tree: parsingTree,
				node: tileAreaNode);

			var areaStartingX = stream.ReadUInt16();
			var areaStartingY = stream.ReadUInt16();
			var areaZ = stream.ReadByte();

			foreach (var tileNode in tileAreaNode.Children) {
				ParseTileNode(
					parsingTree: parsingTree,
					tileNode: tileNode,
					areaStartingX: areaStartingX,
					areaStartingY: areaStartingY,
					areaZ: areaZ);
			}
		}

		private static void ParseTileNode(
			ParsingTree parsingTree,
			ParsingNode tileNode,
			UInt16 areaStartingX,
			UInt16 areaStartingY,
			Byte areaZ
			) {
			if (tileNode.Type != NodeType.NormalTile && tileNode.Type != NodeType.HouseTile)
				throw new MalformedTileAreaNodeException("Unknow tile area node type.");

			var stream = new ParsingStream(parsingTree, tileNode);

			var tileX = areaStartingX + stream.ReadByte();
			var tileY = areaStartingY + stream.ReadByte();

			Tile tile = null;
			if (tileNode.Type == NodeType.HouseTile)
				tile = ParseHouseTile(ref stream, (ushort)tileX, (ushort)tileY, areaZ); // Improve this, remove casts
			else
				tile = ParseNormalTile(ref stream, (ushort)tileX, (ushort)tileY, areaZ);

			throw new NotImplementedException();
		}

		private static Tile ParseHouseTile(ref ParsingStream stream, UInt16 tileX, UInt16 tileY, byte tileZ) {
			var houseId = stream.ReadUInt32();
			var house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
			var tile = HouseTile.CreateTileAndAddItToHouse(tileX, tileY, tileZ, house);

			return tile;
		}

		private static Tile ParseNormalTile(ref ParsingStream stream, UInt16 tileX, UInt16 tileY, byte tileZ) {
			throw new NotImplementedException();
		}		

		private static TileFlags ParseTileFlags(ref ParsingStream stream) {
			var flags = (TileFlags)stream.ReadUInt32();
			var tileFlags = TileFlags.None;

			if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.NoLogout))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.NoLogout);

			if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.NoPvpZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.NoPvpZone);
			else if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.PvpZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.PvpZone);
			else if (TileFlagsExtensions.FlagsAreSet(flags, (TileFlags)OTBMTileFlag.ProtectionZone))
				TileFlagsExtensions.AddFlags(tileFlags, (TileFlags)OTBMTileFlag.ProtectionZone);

			return tileFlags;
		}
	}
}