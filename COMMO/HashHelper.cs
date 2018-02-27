namespace COMMO {

    public static class HashHelper {
        public const int Start = 1610612741;

        public static int CombineHashCode<T>(this int hashCode, T arg) {
            unchecked {
                return 16777619 * hashCode + arg.GetHashCode();
            }
        }
    }
}