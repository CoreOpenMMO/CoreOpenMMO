namespace COMMO {

    public static class FloatExtensions {

        public static bool IsNanOrInfinity(this float f) => float.IsNaN(f) || float.IsInfinity(f);
    }
}