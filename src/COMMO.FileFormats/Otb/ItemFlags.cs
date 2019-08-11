using System;

namespace COMMO.FileFormats.Otb
{
    [Flags]
    public enum ItemFlags : uint
    {
        NotWalkable = 1,

        BlockProjectile = 2,

        BlockPathFinding = 4,

        HasHeight = 8,

        Useable = 16,

        Pickupable = 32,

        Moveable = 64,

        Stackable = 128,

        AlwaysOnTop = 8192,

        Readable = 16384,

        Rotatable = 32768,

        Hangable = 65536,

        Vertical = 131072,

        Horizontal = 262144,

        AllowDistanceRead = 1048576,

        LookThrough = 8388608,

        Animation = 16777216,

        WalkStack = 33554432
    }
}