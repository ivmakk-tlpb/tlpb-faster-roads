using FasterRoads;
using FasterRoads.Patches;
using Xunit;

public class StaminaPatchesTests : GameTestBase
{
    private readonly Scene scene = new Scene(playerId: 7);

    // One StaminaView.Update: the prefix, then the game reads the sum and sets it to 0.
    private float Frame(float gameChange = 0f)
    {
        scene.Stamina.staminaExertionRateChange = gameChange;
        StaminaPatches.Prefix(scene.Stamina);
        return scene.Stamina.staminaExertionRateChange;
    }

    [Fact]
    public void On_a_road_the_prefix_takes_the_drain_away_on_every_other_frame_with_the_default()
    {
        scene.OnRoad();

        Assert.Equal(new[] { -1f, 0f, -1f, 0f }, new[] { Frame(), Frame(), Frame(), Frame() });
    }

    [Fact]
    public void Off_a_road_the_prefix_adds_nothing()
    {
        scene.HudFrame(10f);

        StaminaPatches.Prefix(scene.Stamina);

        Assert.Equal(0f, scene.Stamina.staminaExertionRateChange);
    }

    [Fact]
    public void The_prefix_adds_to_a_game_effect_that_is_already_in_the_sum()
    {
        scene.OnRoad();

        float mean = (Frame(0.2f) + Frame(0.2f)) / 2f;

        Assert.Equal(-0.3f, mean, 4);
    }

    [Fact]
    public void A_drain_multiplier_of_zero_removes_the_drain_of_moving()
    {
        Plugin.StaminaDrainMultiplier.Value = 0f;
        scene.OnRoad();

        Assert.Equal(new[] { -1f, -1f }, new[] { Frame(), Frame() });
    }

    [Fact]
    public void A_creature_on_a_road_loses_stamina_as_in_the_game()
    {
        scene.OnRoad();
        var wolf = scene.CreatureStamina();

        StaminaPatches.Prefix(wolf);

        Assert.Equal(0f, wolf.staminaExertionRateChange);
    }

    [Fact]
    public void A_drain_multiplier_of_one_adds_nothing()
    {
        Plugin.StaminaDrainMultiplier.Value = 1f;
        Plugin.Verbose.Value = true;
        scene.OnRoad();
        Plugin.Log.Debugs.Clear();

        StaminaPatches.Prefix(scene.Stamina);

        Assert.Equal(0f, scene.Stamina.staminaExertionRateChange);
        Assert.Empty(Plugin.Log.Debugs);
    }

    [Fact]
    public void An_error_in_the_prefix_gives_one_warning()
    {
        scene.OnRoad();
        scene.Stamina.ThrowOnAdd = true;

        StaminaPatches.Prefix(scene.Stamina);
        StaminaPatches.Prefix(scene.Stamina);

        Assert.Single(Plugin.Log.Warnings);
    }

    [Fact]
    public void The_first_apply_writes_one_verbose_line_with_the_entity_id()
    {
        Plugin.Verbose.Value = true;
        scene.OnRoad();
        Plugin.Log.Debugs.Clear();

        StaminaPatches.Prefix(scene.Stamina);
        StaminaPatches.Prefix(scene.Stamina);

        Assert.Equal(new[] { "Road stamina boost applied to entity 7" }, Plugin.Log.Debugs);
    }
}
