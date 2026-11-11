using System.Globalization;

namespace FasterRoads.Game
{
    // Reads the ground under the local player's feet into the road state. The ground is a road only when it
    // is terrain and the game reports its surface as ROAD or WET_ROAD, as the footsteps of the game read it.
    internal static class RoadProbe
    {
        private static RoadState state = new RoadState();

        public static bool IsOn => state.IsOn;

        public static void Reset()
        {
            state = new RoadState();
        }

        public static void Check(EntityMovementView movement, float time)
        {
            if (!state.ShouldCheck(time)) return;
            var surface = SurfaceType.NONE;
            bool road = movement != null && IsOnRoad(movement, out surface);
            if (state.Update(time, road)) Plugin.Debug(StateLine(road, surface));
        }

        public static bool IsOnRoad(EntityMovementView movement) => IsOnRoad(movement, out _);

        private static bool IsOnRoad(EntityMovementView movement, out SurfaceType surface)
        {
            surface = SurfaceType.NONE;
            var ground = movement.groundObject;
            if (ground == null || ground.layer != ObjectLayer.terrainLayer) return false;
            var world = movement.GetEntity()?.world;
            if (world == null) return false;
            surface = world.GetSurfaceTypeAtTerrainPosition(movement.transform.position);
            return Boost.IsRoadSurface((int)surface);
        }

        private static string StateLine(bool road, SurfaceType surface)
        {
            if (!road) return "Road boost off: surface " + surface;
            return "Road boost on: surface " + surface
                + ", speed x" + Boost.Speed(Plugin.SpeedMultiplier.Value).ToString("0.##", CultureInfo.InvariantCulture)
                + ", stamina drain x" + Boost.Drain(Plugin.StaminaDrainMultiplier.Value).ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
