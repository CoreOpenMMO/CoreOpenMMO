namespace COMMO.Common.Structures
{
    public class Position
    {
        public Position(int x, int y, int z)
        {
            X = (ushort)x;

            Y = (ushort)y;

            Z = (byte)z;
        }

        public ushort X { get; }

        public ushort Y { get; }

        public byte Z { get; }

        public bool IsInventory
        {
            get
            {
                if (X == 65535)
                {
                    if (Y < 64)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

		public byte InventoryIndex => (byte) (Y);

		public bool IsContainer
        {
            get
            {
                if (X == 65535)
                {
                    if (Y >= 64)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

		public byte ContainerId => (byte) (Y - 64);

		public byte ContainerIndex => (byte) (Z);

		public Position Offset(int x, int y, int z) => new Position(X + x, Y + y, Z + z);

		public Position Offset(MoveDirection moveDirection)
        {
            switch (moveDirection)
            {
                case MoveDirection.East:

                    return new Position(X + 1, Y, Z);

                case MoveDirection.NorthEast:

                    return new Position(X + 1, Y - 1, Z);

                case MoveDirection.North:

                    return new Position(X, Y - 1, Z);

                case MoveDirection.NorthWest:

                    return new Position(X - 1, Y - 1, Z);

                case MoveDirection.West:

                    return new Position(X - 1, Y, Z);

                case MoveDirection.SouthWest:

                    return new Position(X - 1, Y + 1, Z);

                case MoveDirection.South:

                    return new Position(X, Y + 1, Z);

                default:

                    return new Position(X + 1, Y + 1, Z);
            }
        }
        
        public Direction ToDirection(Position that)
        {
            int deltaY = that.Y - Y;

            int deltaX = that.X - X;

            if (deltaX < 0)
            {
                return Direction.West;
            }
            else if (deltaX == 0)
            {
                if (deltaY < 0)
                {
                    return Direction.North;
                }
                else if (deltaY > 0)
                {
                    return Direction.South;
                }
            }
            else if (deltaX > 0)
            {
                return Direction.East;
            }

            return Direction.South;
        }

        public MoveDirection ToMoveDirection(Position that)
        {
            int deltaY = that.Y - Y;

            int deltaX = that.X - X;

            if (deltaY < 0)
            {
                if (deltaX < 0)
                {
                    return MoveDirection.NorthWest;
                }
                else if (deltaX == 0)
                {
                    return MoveDirection.North;
                }
                else if (deltaX > 0)
                {
                    return MoveDirection.NorthEast;
                }
            }
            else if (deltaY == 0)
            {
                if (deltaX < 0)
                {
                    return MoveDirection.West;
                }
                else if (deltaX > 0)
                {
                    return MoveDirection.East;
                }
            }
            else if (deltaY > 0)
            {
                if (deltaX < 0)
                {
                    return MoveDirection.SouthWest;
                }
                else if (deltaX == 0)
                {
                    return MoveDirection.South;
                }
                else if (deltaX > 0)
                {
                    return MoveDirection.SouthEast;
                }
            }

            return MoveDirection.South;
        }

        public bool IsNextTo(Position that)
        {
            int deltaZ = that.Z - Z;

            if (deltaZ != 0)
            {
                return false;
            }

            int deltaY = that.Y - Y + deltaZ;

            if (deltaY < -1 || deltaY > 1)
            {
                return false;
            }

            int deltaX = that.X - X + deltaZ;

            if (deltaX < -1 || deltaX > 1)
            {
                return false;
            }

            return true;
        }

        public bool CanSee(Position that)
        {
            int deltaZ = that.Z - Z;

            int deltaY = that.Y - Y + deltaZ;

            int deltaX = that.X - X + deltaZ;

            if (Z >= 8)
            {
                if (deltaZ < -2 || deltaZ > 2)
                {
                    return false;
                }
            }

            if (Z <= 7) 
            {
                if (that.Z >= 8)
                {
                    return false;
                }
            }

            if (deltaX < -8 || deltaX > 9 || deltaY <-6 || deltaY > 7)
            {
                return false;
            }

            return true;
        }

        public bool CanHearSay(Position that)
        {
            int deltaZ = that.Z - Z;

            int deltaY = that.Y - Y;

            int deltaX = that.X - X;

            if (deltaZ != 0 || deltaX < -8 || deltaX > 9 || deltaY < -6 || deltaY > 7)
            {
                return false;
            }

            return true;
        }

        public bool CanHearWhisper(Position that)
        {
            int deltaZ = that.Z - Z;

            int deltaY = that.Y - Y;

            int deltaX = that.X - X;

            if (deltaZ != 0 || deltaX < -1 || deltaX > 1 || deltaY < -1 || deltaY > 1)
            {
                return false;
            }

            return true;
        }

        public bool CanHearYell(Position that)
        {
            int deltaZ = that.Z - Z;

            int deltaY = that.Y - Y + deltaZ;

            int deltaX = that.X - X + deltaZ;

            if (Z >= 8 || that.Z >= 8)
            {
                if (deltaZ != 0)
                {
                    return false;
                }
            }

            if (deltaX < -30 || deltaX > 30 || deltaY < -30 || deltaY > 30)
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Position a, Position b)
        {
            if ( object.ReferenceEquals(a, b) )
            {
                return true;
            }

            if ( a is null || b is null)
            {
                return false;
            }

            return (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z);
        }

		public static bool operator !=(Position a, Position b) => !(a == b);

		public override bool Equals(object position) => Equals(position as Position);

		public bool Equals(Position position)
        {
            if (position == null)
            {
                return false;
            }

            return (X == position.X) && (Y == position.Y) && (Z == position.Z);
        }

        public override int GetHashCode()
        {
            int hashCode = 17;

            hashCode = hashCode * 23 + X.GetHashCode();

            hashCode = hashCode * 23 + Y.GetHashCode();

            hashCode = hashCode * 23 + Z.GetHashCode();

            return hashCode;
        }

        public override string ToString()
        {
            if (IsInventory)
            {
                return "Slot index: " + InventoryIndex;
            }

            if (IsContainer)
            {
                return "Container id: " + ContainerId + " index: " + ContainerIndex;
            }

            return "Coordinate x: " + X + " y: " + Y + " z: " + Z;
        }
    }
}