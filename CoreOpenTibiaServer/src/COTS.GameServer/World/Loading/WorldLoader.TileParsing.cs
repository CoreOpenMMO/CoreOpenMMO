using System;

namespace COTS.GameServer.World.Loading {

    public static partial class WorldLoader {

        public static Tile ParseTileAreaNode(ParsingTree parsingTree, ParsingNode tileAreaNode) {
            if (parsingTree == null)
                throw new ArgumentNullException(nameof(parsingTree));
            if (tileAreaNode == null)
                throw new ArgumentNullException(nameof(tileAreaNode));

            var stream = new WorldParsingStream(
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

            throw new NotImplementedException();
        }

        private static void ParseTileNode(
            ParsingTree parsingTree,
            ParsingNode tileNode,
            UInt16 areaStartingX,
            UInt16 areaStartingY,
            Byte areaZ
            ) {
            if (tileNode.Type != NodeType.NormalTile && tileNode.Type != NodeType.HouseTile)
                throw new MalformedTileAreaNodeException();

            var houseManager = HouseManager.Instance;
            var stream = new WorldParsingStream(parsingTree, tileNode);

            var xOffset = stream.ReadByte();
            var yOffset = stream.ReadByte();
            var tileX = (UInt16)(areaStartingX + xOffset);
            var tileY = (UInt16)(areaStartingY + yOffset);

            Tile tile = null;
            TileFlags tileFlags = TileFlags.None;
            var isHouse = false;

            // Handling the HouseTile case
            if (tileNode.Type == NodeType.HouseTile) {
                UInt32 houseId = stream.ReadUInt32();
                var house = houseManager.CreateHouseOrGetReference(houseId);
                tile = new HouseTile(
                    x: tileX,
                    y: tileY,
                    z: areaZ,
                    house: house);
                house.AddTile((HouseTile)tile);

                isHouse = true;
            }

            // Parsing the tile attributes
            while (!stream.IsOver) {
                var nodeAttribute = (NodeAttribute)stream.ReadByte();
                switch (nodeAttribute) {
                    case NodeAttribute.TileFlags:
                    var otbmFlags = stream.ReadUInt32();
                    tileFlags = UpdateTileFlags(otbmFlags, tileFlags);
                    break;

                    case NodeAttribute.Item:

                    break;

                    default:
                    throw new MalformedTileAreaNodeException();
                }
            }

            throw new NotImplementedException();
        }

        private static TileFlags UpdateTileFlags(UInt32 otbmFlags, TileFlags tileFlags) {
            var updatedFlags = tileFlags;

            // Mutually exclusive flags
            if ((otbmFlags & (uint)OTBMTileFlag.ProtectionZone) != 0)
                updatedFlags |= TileFlags.ProtectionZone;
            else if ((otbmFlags & (uint)OTBMTileFlag.NoPvpZone) != 0)
                updatedFlags |= TileFlags.NoPvpZone;
            else if ((otbmFlags & (uint)OTBMTileFlag.PvpZone) != 0)
                updatedFlags |= TileFlags.PvpZone;

            // Just another flag
            if ((otbmFlags & (uint)OTBMTileFlag.NoLogout) != 0)
                updatedFlags |=  TileFlags.NoLogout;

            return updatedFlags;
        }
    }
}