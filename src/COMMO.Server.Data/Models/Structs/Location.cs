// <copyright file="Location.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using System;
using COMMO.Data.Contracts;

namespace COMMO.Server.Data.Models.Structs
{
    public struct Location
    {
        public int X { get; set; }

        public int Y { get; set; }

        public sbyte Z { get; set; }

        public bool IsUnderground => Z > 7;

        public LocationType Type
        {
            get
            {
                if (X != 0xFFFF)
                {
                    return LocationType.Ground;
                }

                if ((Y & 0x40) != 0)
                {
                    return LocationType.Container;
                }

                return LocationType.Slot;
            }
        }

        public static Location operator +(Location location1, Location location2)
        {
            return new Location
            {
                X = location1.X + location2.X,
                Y = location1.Y + location2.Y,
                Z = (sbyte)(location1.Z + location2.Z)
            };
        }

        public static Location operator -(Location location1, Location location2)
        {
            return new Location
            {
                X = location2.X - location1.X,
                Y = location2.Y - location1.Y,
                Z = (sbyte)(location2.Z - location1.Z)
            };
        }

        public Slot Slot => (Slot)Convert.ToByte(Y);

        public byte Container => Convert.ToByte(Y - 0x40);

        public sbyte ContainerPosition
        {
            get
            {
                return Convert.ToSByte(Z);
            }

            set
            {
                Z = value;
            }
        }

        public int MaxValueIn2D => Math.Max(Math.Abs(X), Math.Abs(Y));

        public int MaxValueIn3D => Math.Max(MaxValueIn2D, Math.Abs(Z));

        public override string ToString()
        {
            return $"[{X}, {Y}, {Z}]";
        }

        public override bool Equals(object obj)
        {
            return obj is Location && this == (Location)obj;
        }

        public override int GetHashCode()
        {
            int hash = 13;

            hash = (hash * 7) + X.GetHashCode();
            hash = (hash * 7) + Y.GetHashCode();
            hash = (hash * 7) + Z.GetHashCode();

            return hash;
        }

        public static bool operator ==(Location origin, Location targetLocation)
        {
            try
            {
                return origin.X == targetLocation.X && origin.Y == targetLocation.Y && origin.Z == targetLocation.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator !=(Location origin, Location targetLocation)
        {
            try
            {
                return origin.X != targetLocation.X || origin.Y != targetLocation.Y || origin.Z != targetLocation.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator >(Location first, Location second)
        {
            try
            {
                return first.X > second.X || first.Y > second.Y || first.Z > second.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static bool operator <(Location first, Location second)
        {
            try
            {
                return first.X < second.X || first.Y < second.Y || first.Z < second.Z;
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }

        public static long[] GetOffsetBetween(Location origin, Location targetLocation)
        {
            return new[] {
                (long)origin.X - targetLocation.X,
                (long)origin.Y - targetLocation.Y,
                (long)origin.Z - targetLocation.Z
            };
        }

        public Direction DirectionTo(Location targetLocation, bool returnDiagonals = false)
        {
            var locationDiff = this - targetLocation;

            if (!returnDiagonals)
            {
                if (locationDiff.X < 0)
                {
                    return Direction.West;
                }

                if (locationDiff.X > 0)
                {
                    return Direction.East;
                }

                return locationDiff.Y < 0 ? Direction.North : Direction.South;
            }

            if (locationDiff.X < 0)
            {
                if (locationDiff.Y < 0)
                {
                    return Direction.NorthWest;
                }

                return locationDiff.Y > 0 ? Direction.SouthWest : Direction.West;
            }

            if (locationDiff.X > 0)
            {
                if (locationDiff.Y < 0)
                {
                    return Direction.NorthEast;
                }

                return locationDiff.Y > 0 ? Direction.SouthEast : Direction.East;
            }

            return locationDiff.Y < 0 ? Direction.North : Direction.South;
        }
    }
}