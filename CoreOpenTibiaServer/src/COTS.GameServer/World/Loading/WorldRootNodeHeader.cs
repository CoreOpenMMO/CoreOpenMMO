using System;

namespace COTS.GameServer.World.Loading {

    public sealed class WorldRootNodeHeader {
        public readonly UInt32 WorldEncodingVersion;
        public readonly UInt16 WorldWidth;
        public readonly UInt16 WorldHeight;
        public readonly UInt32 ItemEncodingMajorVersion;
        public readonly UInt32 ItemEncodingMinorVersion;

        public WorldRootNodeHeader(
            uint worldEncodingVersion,
            ushort worldWidth,
            ushort worldHeight,
            uint itemEncodingMajorVersion,
            uint itemEncodingMinorVersion
            ) {
            WorldEncodingVersion = worldEncodingVersion;
            WorldWidth = worldWidth;
            WorldHeight = worldHeight;
            ItemEncodingMajorVersion = itemEncodingMajorVersion;
            ItemEncodingMinorVersion = itemEncodingMinorVersion;
        }
    }
}