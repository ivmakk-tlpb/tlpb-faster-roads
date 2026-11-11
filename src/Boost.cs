namespace FasterRoads
{
    // The road boost values from the two multipliers. A value outside its config range is limited to it.
    internal static class Boost
    {
        // SurfaceType.ROAD and SurfaceType.WET_ROAD of the game.
        internal const int RoadSurface = 17;
        internal const int WetRoadSurface = 18;

        public static float Speed(float multiplier) => Clamp(multiplier, 1f, 3f);

        public static float Drain(float multiplier) => Clamp(multiplier, 0f, 1f);

        // The change that the game adds to its movement speed accumulator.
        public static float SpeedChange(float multiplier) => Speed(multiplier) - 1f;

        // The change that the game adds to its stamina exertion rate accumulator.
        public static float DrainChange(float multiplier) => Drain(multiplier) - 1f;

        public static bool ShowTile(float speedMultiplier, float drainMultiplier, bool showIcon)
        {
            return showIcon && (SpeedChange(speedMultiplier) != 0f || DrainChange(drainMultiplier) != 0f);
        }

        public static bool IsRoadSurface(int surface) => surface == RoadSurface || surface == WetRoadSurface;

        private static float Clamp(float value, float min, float max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}
