using System;

namespace COMMO.Common.Structures
{
    [Flags]
    public enum FloorChange
    {
        None = 0,

        Down = 1,

        East = 2,

        North = 4,

        West = 8,

        South = 16,

        NorthEast = North | East,

        NorthWest = North | West,

        SouthWest = South | West,

        SouthEast = South | East
    }
}