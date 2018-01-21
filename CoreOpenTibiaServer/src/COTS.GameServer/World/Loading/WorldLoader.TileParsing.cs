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
            if (tileNode.Type == NodeType.NormalTile)
                throw new NotImplementedException();
            else if (tileNode.Type == NodeType.HouseTile)
                ParseHouseTile(parsingTree, tileNode, areaStartingX, areaStartingY, areaZ);
            else
                throw new MalformedTileNodeException();

            //if (tileNode.Type != NodeType.NormalTile && tileNode.Type != NodeType.HouseTile)
            //    throw new MalformedTileAreaNodeException();
            
            //var stream = new WorldParsingStream(parsingTree, tileNode);

            //var xOffset = stream.ReadByte();
            //var yOffset = stream.ReadByte();
            //var tileX = (UInt16)(areaStartingX + xOffset);
            //var tileY = (UInt16)(areaStartingY + yOffset);

            //ParseHouseTile(
            //    stream: ref stream,
            //    x: tileX,
            //    y: tileY,
            //    )

            //Tile tile = null;
            //Item item = null;
            //Item groundItem = null;
            //TileFlags tileFlags = TileFlags.None;
            //var isHouse = false;

            //// Handling the HouseTile case
            //if (tileNode.Type == NodeType.HouseTile) {
            //    UInt32 houseId = stream.ReadUInt32();
            //    var house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
            //    tile = new HouseTile(
            //        x: tileX,
            //        y: tileY,
            //        z: areaZ,
            //        house: house);
            //    house.AddTile((HouseTile)tile);

            //    isHouse = true;
            //}

            //// Parsing the tile attributes
            //while (!stream.IsOver) {
            //    var nodeAttribute = (NodeAttribute)stream.ReadByte();

            //    if (nodeAttribute == NodeAttribute.TileFlags) {
            //        var otbmFlags = stream.ReadUInt32();
            //        tileFlags = UpdateTileFlags(otbmFlags, tileFlags);
            //        continue;
            //    }

            //    if (nodeAttribute == NodeAttribute.Item) {
            //        var itemId = stream.ReadUInt16();
            //        item = Item.CreateFromId(itemId);

            //        if (isHouse && item.IsMoveable)
            //            throw new MoveableItemInHouseException();

            //        if (tile != null) {
            //            tile.AddInternalThing(item);
            //            item.StartDecaying();
            //            item.LoadedFromMap = true;
            //        } else if (item.IsGroundTile) {
            //            groundItem = item;
            //        } else {
            //            tile = CreateTile(
            //                ground: groundItem,
            //                item: item,
            //                x: tileX,
            //                y: tileY, 
            //                z: areaZ);

            //            tile.AddInternalThing(item);
            //            item.StartDecaying();
            //            item.LoadedFromMap = true;
            //        }

            //        continue;
            //    }

            //    // At this point, we are only expecting tile flags or items
            //    throw new MalformedTileAreaNodeException();
            //}
        }

        private static void ParseHouseTile(ParsingTree parsingTree, ParsingNode tileNode, ushort areaStartingX, ushort areaStartingY, byte areaZ) {
            var stream = new WorldParsingStream(parsingTree, tileNode);

            // Calculating the tile's absolute coordinates
            var xOffset = stream.ReadByte();
            var yOffset = stream.ReadByte();
            var tileX = (UInt16)(areaStartingX + xOffset);
            var tileY = (UInt16)(areaStartingY + yOffset);

            // Getting reference to house 
            var houseId = stream.ReadUInt32();
            var house = HouseManager.Instance.CreateHouseOrGetReference(houseId);
            var tile = new HouseTile(
                    x: tileX,
                    y: tileY,
                    z: areaZ,
                    house: house);
            house.AddTile(tile);


            TileFlags updatedTileFlags = TileFlags.None;

            // Parsing the tile attributes
            while (!stream.IsOver) {
                var nodeAttribute = (NodeAttribute)stream.ReadByte();

                if (nodeAttribute == NodeAttribute.TileFlags) {
                    var otbmFlags = stream.ReadUInt32();
                    updatedTileFlags = UpdateTileFlags(otbmFlags, updatedTileFlags);
                    continue;
                }

                if (nodeAttribute == NodeAttribute.Item) {
                    var itemId = stream.ReadUInt16();
                    var item = Item.CreateFromId(itemId);

                    // No idea why, but the original CPP version didn't like "moveable items"
                    // inside houses
                    if (item.IsMoveable)
                        throw new MoveableItemInHouseException();

                    tile.AddInternalThing(item);
                    item.StartDecaying();
                    item.LoadedFromMap = true;
                }

                // At this point, we are only expecting tile flags or items
                throw new MalformedTileAreaNodeException();
            }

            // Updating flags
            tile.Flags = tile.Flags.AddFlags(updatedTileFlags);
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
                updatedFlags |= TileFlags.NoLogout;

            return updatedFlags;
        }
    }
}