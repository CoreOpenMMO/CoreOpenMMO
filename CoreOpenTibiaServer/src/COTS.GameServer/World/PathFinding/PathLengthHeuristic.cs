using System;
using System.Runtime.CompilerServices;

namespace COTS.GameServer.World.PathFinding {

    public static class PathLengthHeuristic {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EstimateDistance(AStartNode lhs, AStartNode rhs) {
            return
                Math.Abs(lhs.X - rhs.X) +
                Math.Abs(lhs.Y - rhs.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EstimateDistance(in PathCoordinate lhs, in PathCoordinate rhs) {
            return
                Math.Abs(lhs.X - rhs.X) +
                Math.Abs(lhs.Y - rhs.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EstimateDistance(in Position lhs, in Position rhs) {
            return
                Math.Abs(lhs.X - rhs.X) +
                Math.Abs(lhs.Y - rhs.Y);
        }
    }
}