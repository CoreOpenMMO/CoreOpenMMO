using COMMO.GameServer.Items;
using COMMO.GameServer.OTBParsing;
using System;
using System.Collections.Generic;

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

			var areaStartPosition = new Coordinate(
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
			in Coordinate tilesAreaStartPosition,
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
			House house = null;
			if (tileNode.Type == OTBNodeType.HouseTile) {
				var houseId = stream.ReadUInt32();
				house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
			}

			// Parsing the tile attributes
			var tileFlags = TileFlags.None;
			var tilesItems = new List<Item>();
			while (!stream.IsOver) {
				var attribute = (TFSWorldNodeAttribute)stream.ReadByte();
				switch (attribute) {
					case TFSWorldNodeAttribute.TileFlags:
					var newFlags = (TFSTileFlag)stream.ReadUInt32();
					tileFlags = UpdateTileFlags(tileFlags, newFlags);
					break;

					case TFSWorldNodeAttribute.Item:
#warning Not implemented -- Halp Ratazana
					break;

					default:
					throw new TFSWorldLoadingException("TFS just threw a exception here, so shall we.");
				}

				throw new NotImplementedException();
			}

			// Parsing tile's items stored as child
			foreach (var itemNode in tileNode.Children) {
#warning Not implemented -- Halp Ratazana
			}

			// Finally, we collected all the data and create the tile,
			// adding it to a house, if necessary
			var tile = new Tile(
				position: tilePosition,
				flags: tileFlags,
				belongsToHouse: house != null,
				items: tilesItems);

			house?.AddTile(tile);
			world.AddTile(tile);
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