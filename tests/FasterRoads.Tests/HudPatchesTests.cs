using FasterRoads;
using FasterRoads.Game;
using Xunit;

public class HudPatchesTests : GameTestBase
{
    private readonly Scene scene = new Scene(playerId: 7);

    [Fact]
    public void The_postfix_keeps_the_id_of_the_hud_player()
    {
        scene.HudFrame(10f);

        Assert.Equal(7, PlayerViews.Id);
    }

    [Fact]
    public void The_postfix_runs_the_road_check_on_each_call_also_with_no_movement()
    {
        scene.HudFrame(10f);
        scene.HudFrame(10.1f);
        scene.HudFrame(10.2f);

        Assert.Equal(3, scene.World.SurfaceLookups.Count);
    }

    [Fact]
    public void The_postfix_turns_the_boost_on_while_the_player_stands_on_a_road()
    {
        scene.OnRoad();

        Assert.True(RoadProbe.IsOn);
    }

    [Fact]
    public void A_hud_with_no_player_turns_the_boost_off_after_the_grace_time()
    {
        scene.OnRoad(10f);
        scene.Hud.player = null;
        scene.HudFrame(10.6f);

        Assert.False(RoadProbe.IsOn);
        Assert.Equal(-1, PlayerViews.Id);
    }

    [Fact]
    public void Another_player_starts_with_the_boost_off()
    {
        scene.OnRoad(10f);
        var other = new Scene(playerId: 9);
        other.HudFrame(10.1f);

        Assert.False(RoadProbe.IsOn);
        Assert.Equal(9, PlayerViews.Id);
    }

    [Fact]
    public void An_error_in_the_postfix_gives_one_warning()
    {
        scene.Player.Components.Insert(0, new BrokenStamina());
        scene.HudFrame(10f);
        scene.HudFrame(10.1f);

        Assert.Single(Plugin.Log.Warnings);
    }

    private sealed class BrokenStamina : IStamina
    {
        public IntPtr Pointer => throw new InvalidOperationException("broken");
    }
}
