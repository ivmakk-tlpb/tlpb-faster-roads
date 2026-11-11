using FasterRoads;
using FasterRoads.Patches;
using Xunit;

public class MovePatchesTests : GameTestBase
{
    private readonly Scene scene = new Scene(playerId: 7);

    [Fact]
    public void On_a_road_the_prefix_adds_the_speed_change_to_the_player_movement()
    {
        scene.OnRoad();

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0.33f, scene.Movement.movementSpeedChange, 4);
    }

    [Fact]
    public void With_no_HUD_update_the_boost_goes_after_the_player_leaves_the_road()
    {
        scene.OnRoad(10f);
        scene.World.Surface = SurfaceType.GRASS;
        UnityEngine.Time.time = 11f;

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0f, scene.Movement.movementSpeedChange);
    }

    [Fact]
    public void With_no_HUD_update_the_boost_comes_when_the_player_runs_onto_a_road()
    {
        scene.HudFrame(10f);
        scene.World.Surface = SurfaceType.ROAD;
        UnityEngine.Time.time = 10.2f;

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0.33f, scene.Movement.movementSpeedChange, 4);
    }

    [Fact]
    public void Off_a_road_the_prefix_adds_nothing()
    {
        scene.HudFrame(10f);

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0f, scene.Movement.movementSpeedChange);
    }

    [Fact]
    public void The_prefix_adds_to_a_game_effect_that_is_already_in_the_sum()
    {
        scene.OnRoad();
        scene.Movement.movementSpeedChange = -0.3f;

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0.03f, scene.Movement.movementSpeedChange, 4);
    }

    [Fact]
    public void The_prefix_adds_nothing_while_the_movement_is_off()
    {
        scene.OnRoad();
        scene.Movement.movementEnabled.Value = false;

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0f, scene.Movement.movementSpeedChange);
    }

    [Fact]
    public void The_prefix_adds_nothing_while_the_player_climbs_over()
    {
        scene.OnRoad();
        scene.Movement.climbingOver = true;

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0f, scene.Movement.movementSpeedChange);
    }

    [Fact]
    public void The_prefix_adds_nothing_while_the_player_lands()
    {
        scene.OnRoad();
        scene.Movement.landing = true;

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0f, scene.Movement.movementSpeedChange);
    }

    [Fact]
    public void A_creature_on_a_road_moves_as_in_the_game()
    {
        scene.OnRoad();
        var wolf = scene.CreatureMovement();

        MovePatches.Prefix(wolf);

        Assert.Equal(0f, wolf.movementSpeedChange);
    }

    [Fact]
    public void A_speed_multiplier_of_one_adds_nothing()
    {
        Plugin.SpeedMultiplier.Value = 1f;
        Plugin.Verbose.Value = true;
        scene.OnRoad();
        Plugin.Log.Debugs.Clear();

        MovePatches.Prefix(scene.Movement);

        Assert.Equal(0f, scene.Movement.movementSpeedChange);
        Assert.Empty(Plugin.Log.Debugs);
    }

    [Fact]
    public void An_error_in_the_prefix_gives_one_warning()
    {
        scene.OnRoad();
        scene.Movement.movementEnabled = null;

        MovePatches.Prefix(scene.Movement);
        MovePatches.Prefix(scene.Movement);

        Assert.Single(Plugin.Log.Warnings);
    }

    [Fact]
    public void The_first_apply_writes_one_verbose_line_with_the_entity_id()
    {
        Plugin.Verbose.Value = true;
        scene.OnRoad();
        Plugin.Log.Debugs.Clear();

        MovePatches.Prefix(scene.Movement);
        MovePatches.Prefix(scene.Movement);

        Assert.Equal(new[] { "Road speed boost applied to entity 7" }, Plugin.Log.Debugs);
    }
}
