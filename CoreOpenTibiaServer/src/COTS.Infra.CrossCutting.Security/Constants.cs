namespace COTS.Infra.CrossCutting.Security
{
    /// <summary>
    /// Some constant-values.
    /// Same as for TFS 1.2/1
    /// </summary>
    public static class Constants
    {
        public const ushort NetworkMessageSizeMax = 24590;
        public const ushort NetworkMessageErrorSizeMax = NetworkMessageSizeMax - 16;

        public const int OutputMessagePoolSize = 100;
        public const int OutputMessagePoolExpansionSize = 10;

        public const int NetworkMessagePoolSize = 100;
        public const int NetworkMessagePoolExpansionSize = 10;
    }
}