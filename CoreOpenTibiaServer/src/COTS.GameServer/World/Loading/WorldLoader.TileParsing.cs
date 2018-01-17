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
                ParseTileNode(parsingTree, tileNode, areaStartingX, areaStartingY);
            }

            throw new NotImplementedException();
        }

        private static void ParseTileNode(
            ParsingTree parsingTree,
            ParsingNode tileNode,
            UInt16 areaStartingX,
            UInt16 areaStartingY
            ) {
            if (tileNode.Type != NodeType.NormalTile && tileNode.Type != NodeType.HouseTile)
                throw new MalformedTileAreaNodeException();

            var stream = new WorldParsingStream(parsingTree, tileNode);

            var xCoordinateOffset = stream.ReadByte();
            var yCoordinateOffset = stream.ReadByte();
            var tileXCoordinate = (UInt16)(areaStartingX + xCoordinateOffset);
            var tileYCoordiante = (UInt16)(areaStartingY + yCoordinateOffset);

            if (tileNode.Type == NodeType.HouseTile) {
                UInt32 houseId = stream.ReadUInt32();
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}