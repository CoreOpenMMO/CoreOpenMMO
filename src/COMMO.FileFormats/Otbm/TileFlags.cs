using System;

namespace COMMO.FileFormats.Otbm
{
    [Flags]
    public enum TileFlags : uint
    {
        ProtectionZone = 1,

        NoPvpZone = 4,

        NoLogoutZone = 8,

        PvpZone = 16,

        Refresh = 32
    }
}