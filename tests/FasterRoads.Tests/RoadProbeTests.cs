using FasterRoads;
using FasterRoads.Game;
using UnityEngine;
using Xunit;

public class RoadProbeTests : GameTestBase
{
    private readonly Scene scene = new Scene();

    [Fact]
    public void No_ground_object_is_no_road()
    {
        scene.World.Surface = SurfaceType.ROAD;
        scene.Movement.groundObject = null;

        Assert.False(RoadProbe.IsOnRoad(scene.Movement));
    }

    [Fact]
    public void A_destroyed_ground_object_is_no_road()
    {
        scene.World.Surface = SurfaceType.ROAD;
        scene.Terrain.Destroyed = true;

        Assert.False(RoadProbe.IsOnRoad(scene.Movement));
    }

    [Fact]
    public void A_ground_object_off_the_terrain_layer_is_no_road()
    {
        scene.World.Surface = SurfaceType.ROAD;
        scene.Movement.groundObject = new GameObject { layer = 3 };

        Assert.False(RoadProbe.IsOnRoad(scene.Movement));
        Assert.Empty(scene.World.SurfaceLookups);
    }

    [Theory]
    [InlineData(SurfaceType.ROAD, true)]
    [InlineData(SurfaceType.WET_ROAD, true)]
    [InlineData(SurfaceType.DIRT, false)]
    [InlineData(SurfaceType.GRASS, false)]
    public void The_terrain_surface_under_the_feet_decides(SurfaceType surface, bool road)
    {
        scene.World.Surface = surface;
        scene.Movement.transform.position = new Vector3(1f, 2f, 3f);

        Assert.Equal(road, RoadProbe.IsOnRoad(scene.Movement));
        Assert.Equal(new Vector3(1f, 2f, 3f), scene.World.SurfaceLookups.Single());
    }

    [Fact]
    public void A_movement_with_no_entity_is_no_road()
    {
        scene.World.Surface = SurfaceType.ROAD;
        scene.Movement.entity = null;

        Assert.False(RoadProbe.IsOnRoad(scene.Movement));
    }

    [Fact]
    public void The_probe_asks_the_world_at_most_once_per_tenth_of_a_second()
    {
        RoadProbe.Check(scene.Movement, 10f);
        RoadProbe.Check(scene.Movement, 10.05f);
        RoadProbe.Check(scene.Movement, 10.1f);

        Assert.Equal(2, scene.World.SurfaceLookups.Count);
    }

    [Fact]
    public void The_probe_feeds_the_road_state()
    {
        scene.World.Surface = SurfaceType.ROAD;
        RoadProbe.Check(scene.Movement, 10f);
        Assert.True(RoadProbe.IsOn);

        scene.World.Surface = SurfaceType.GRASS;
        RoadProbe.Check(scene.Movement, 10.6f);
        Assert.False(RoadProbe.IsOn);
    }

    [Fact]
    public void No_movement_is_no_road()
    {
        RoadProbe.Check(null, 10f);

        Assert.False(RoadProbe.IsOn);
    }

    [Fact]
    public void A_change_of_the_road_state_writes_one_verbose_line_with_the_surface_and_the_multipliers()
    {
        Plugin.Verbose.Value = true;
        scene.World.Surface = SurfaceType.WET_ROAD;
        RoadProbe.Check(scene.Movement, 10f);
        RoadProbe.Check(scene.Movement, 10.1f);
        scene.World.Surface = SurfaceType.GRASS;
        RoadProbe.Check(scene.Movement, 10.6f);

        Assert.Equal(new[]
        {
            "Road boost on: surface WET_ROAD, speed x1.33, stamina drain x0.5",
            "Road boost off: surface GRASS",
        }, Plugin.Log.Debugs);
    }
}
