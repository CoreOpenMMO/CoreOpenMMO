using COMMO.GameServer.OTBParsing;
using System;

namespace COMMO.GameServer.World.TFSLoading {

	public static partial class TFSWorldLoader {

		private static void ParseTileAreaNode(OTBNode tileAreaNode, World world) {
			if (tileAreaNode == null)
				throw new ArgumentNullException(nameof(tileAreaNode));
			if (tileAreaNode.Type != OTBNodeType.TileArea)
				throw new TFSWorldLoadingException();

			var stream = new OTBParsingStream(tileAreaNode.Data.Span);

			var areaStartX = stream.ReadUInt16();
			var areaStartY = stream.ReadUInt16();
			var areaZ = stream.ReadByte();

			var areaStartPosition = new Position(
				x: areaStartX,
				y: areaStartY,
				z: areaZ);

			foreach (var tileNode in tileAreaNode.Children) {
				ParseTileNode(
					tilesAreaStartPosition: areaStartPosition,
					tileNode: tileNode,
					world: world);
			}
		}

		private static void ParseTileNode(
			in Position tilesAreaStartPosition,
			OTBNode tileNode,
			World world
			) {
			if (tileNode == null)
				throw new ArgumentNullException(nameof(tileNode));
			if (world == null)
				throw new ArgumentNullException(nameof(world));
			if (tileNode.Type != OTBNodeType.HouseTile && tileNode.Type != OTBNodeType.NormalTile)
				throw new TFSWorldLoadingException();

			var stream = new OTBParsingStream(tileNode.Data.Span);

			// Finding the tiles "absolute coordinates"
			var xOffset = stream.ReadUInt16();
			var yOffset = stream.ReadUInt16();
			var tilePosition = tilesAreaStartPosition.Translate(
				xOffset: xOffset,
				yOffset: yOffset);

			// Checking whether the tile belongs to a house
			var tileBelongsToAHouse = tileNode.Type == OTBNodeType.HouseTile;

			var tileFlags = TileFlags.None;

			// Parsing the tile attributes
			while (!stream.IsOver) {
				var attribute = (TFSWorldNodeAttribute)stream.ReadByte();
				switch (attribute) {
					case TFSWorldNodeAttribute.TileFlags:
					var newFlags = (TFSTileFlag)stream.ReadUInt32();
					tileFlags = UpdateTileFlags(tileFlags, newFlags);
					break;

					case TFSWorldNodeAttribute.Item:
					// Do stuff
					break;

					default:
					throw new TFSWorldLoadingException("TFS just threw a exception here, so shall we.");
				}


				throw new NotImplementedException();
			}

			// Finally, we create the actual tile
			var tile = new Tile(
				position: tilePosition,
				flags: tileFlags,
				belongsToHouse: tileBelongsToAHouse);

			// Add it to a house, if necessary
			if (tileBelongsToAHouse) {
				var houseId = stream.ReadUInt32();
				HouseManager.Instance
					.CreateHouseOrGetReference(houseId)
					.AddTile(tile);
			}
		}

		private static TileFlags UpdateTileFlags(TileFlags oldFlags, TFSTileFlag newFlags) {
			if ((newFlags & TFSTileFlag.NoLogout) != 0)
				oldFlags |= TileFlags.NoLogout;

			// I think we should throw if a tile contains contradictory flags, instead of just
			// ignoring them like tfs does...
			if ((newFlags & TFSTileFlag.ProtectionZone) != 0)
				oldFlags |= TileFlags.ProtectionZone;
			else if ((newFlags & TFSTileFlag.NoPvpZone) != 0)
				oldFlags |= TileFlags.NoPvpZone;
			else if ((newFlags & TFSTileFlag.PvpZone) != 0)
				oldFlags |= TileFlags.PvpZone;

			return oldFlags;
		}
	}
}