using System;

namespace FasterRoads
{
    // The tooltip texts of the road boost tile. A multiplier that gives 0 % has no line.
    internal static class RoadText
    {
        public const string Title = "On the Road";
        public const string Description = "Roads make travel easier.";

        public static string SpeedLine(float multiplier)
        {
            int percent = WholePercent(Boost.SpeedChange(multiplier));
            return percent == 0 ? null : "Movement speed is " + percent + "% faster";
        }

        public static string DrainLine(float multiplier)
        {
            int percent = WholePercent(-Boost.DrainChange(multiplier));
            return percent == 0 ? null : "Stamina depletes " + percent + "% slower";
        }

        private static int WholePercent(float change)
        {
            return (int)Math.Round(change * 100.0, MidpointRounding.AwayFromZero);
        }
    }
}
