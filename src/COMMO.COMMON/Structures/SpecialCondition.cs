using System;

namespace COMMO.Common.Structures
{
    [Flags]
    public enum SpecialCondition : ushort
    {
        None = 0,

        Poisoned = 1,

        Burning = 2,

        Electrified = 4,

        Drunk = 8,

        MagicShield = 16,

        Slowed = 32,

        Haste = 64,

        LogoutBlock = 128,

        Drowning = 256,

        Freezing = 512,

        Dazzled = 1024,

        Cursed = 2048,

        Bleeding = 4096,

        ProtectionZoneBlock = 8192,

        ProtectionZone = 16384
    }
}