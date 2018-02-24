using System;

namespace COTS.GameServer.World {

    public sealed class WorldHeader {
        public readonly UInt32 WorldEncodingVersion;
        public readonly UInt16 WorldWidth;
        public readonly UInt16 WorldHeight;
        public readonly UInt32 ItemEncodingMajorVersion;
        public readonly UInt32 ItemEncodingMinorVersion;

        public WorldHeader(
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