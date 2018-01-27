namespace COTS.GameServer.World.PathFinding {

    /// <summary>
    /// This class contains the parameters utilized by the path finding algorithm.
    /// Defaut values were copied from the C++ reference source.
    /// </summary>
    public sealed class PathFindingParameters {
        public readonly bool PerformenceFullPathSearch = true;
        public readonly bool RequiresClearSight = true;
        public readonly bool AllowDiagonalMovement = true;
        public readonly int MaximumSearchDistance = 0;
        public readonly int MinimumTargetDistance = -1;
        public readonly int MaximumTargetDistance = -1;
    }
}