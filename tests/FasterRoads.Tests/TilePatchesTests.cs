using FasterRoads;
using FasterRoads.Patches;
using Xunit;

public class TilePatchesTests : GameTestBase
{
    private readonly Scene scene = new Scene(playerId: 7);

    // One HUD update: the prefix, the game's call of GetActiveStatusEffects with the postfix, and the finalizer.
    private List<ActiveStatusEffect> HudCall(StatusEffectsView view = null)
    {
        var result = new Il2CppSystem.Collections.Generic.List<ActiveStatusEffect>();
        TilePatches.HudPrefix();
        TilePatches.ActiveEffectsPostfix(view ?? scene.Status, result);
        TilePatches.HudFinalizer();
        return result.Items;
    }

    [Fact]
    public void The_hud_prefix_sets_the_flag_and_the_finalizer_clears_it()
    {
        TilePatches.HudPrefix();
        Assert.True(TilePatches.InHud);

        TilePatches.HudFinalizer();
        Assert.False(TilePatches.InHud);
    }

    [Fact]
    public void On_a_road_the_hud_list_gets_the_road_boost_entry()
    {
        scene.OnRoad();

        Assert.Equal(new[] { TilePatches.Entry }, HudCall());
    }

    [Fact]
    public void Off_a_road_the_hud_list_gets_no_entry()
    {
        scene.HudFrame(10f);

        Assert.Empty(HudCall());
    }

    [Fact]
    public void A_call_outside_the_hud_update_gets_no_entry()
    {
        scene.OnRoad();
        var result = new Il2CppSystem.Collections.Generic.List<ActiveStatusEffect>();

        TilePatches.ActiveEffectsPostfix(scene.Status, result);

        Assert.Empty(result.Items);
    }

    [Fact]
    public void The_status_effects_of_another_entity_get_no_entry()
    {
        scene.OnRoad();
        var other = new StatusEffectsView { entity = new Entity { id = 50 } };

        Assert.Empty(HudCall(other));
    }

    [Fact]
    public void The_icon_setting_is_read_on_each_call()
    {
        scene.OnRoad();
        Plugin.ShowRoadIcon.Value = false;
        Assert.Empty(HudCall());

        Plugin.ShowRoadIcon.Value = true;
        Assert.Single(HudCall());
    }

    [Fact]
    public void Two_multipliers_of_one_show_no_entry()
    {
        Plugin.SpeedMultiplier.Value = 1f;
        Plugin.StaminaDrainMultiplier.Value = 1f;
        scene.OnRoad();

        Assert.Empty(HudCall());
    }

    [Fact]
    public void The_same_entry_comes_back_each_frame()
    {
        scene.OnRoad();

        Assert.Same(HudCall().Single(), HudCall().Single());
    }

    [Fact]
    public void The_entry_is_for_display_only()
    {
        var entry = TilePatches.Entry;

        Assert.Equal(StatusEffectType.NONE, entry.statusEffectType);
        Assert.Null(entry.statusEffectDefinition);
        Assert.Equal(0, entry.itemId);
        Assert.Equal(0, entry.substanceId);
        Assert.Equal(0f, entry.duration);
        Assert.Equal(0f, entry.rate);
        Assert.Equal(0f, entry.movementSpeedChange);
        Assert.Equal(0f, entry.staminaExertionRateChange);
        Assert.Empty(entry.activeInjuryIds.Items);
    }

    [Fact]
    public void An_error_in_the_postfix_gives_one_warning()
    {
        scene.OnRoad();
        TilePatches.HudPrefix();
        TilePatches.ActiveEffectsPostfix(scene.Status, null);
        TilePatches.ActiveEffectsPostfix(scene.Status, null);
        TilePatches.HudFinalizer();

        Assert.Single(Plugin.Log.Warnings);
    }

    [Fact]
    public void The_tile_of_the_entry_shows_the_green_movement_icon_and_the_up_arrow()
    {
        var display = new StatusEffectDisplayUI { statusEffect = TilePatches.Entry };

        TilePatches.CreatedPostfix(display);

        Assert.True(display.statusImage.enabled);
        Assert.True(display.indicatorImage.enabled);
        Assert.Equal(IconType.MOVEMENT, display.statusImage.sprite.Icon);
        Assert.Equal(ColorUtil.staminaColor, display.statusImage.color);
        Assert.Equal(IconType.INCREASING, display.indicatorImage.sprite.Icon);
        Assert.True(display.positive);
    }

    [Fact]
    public void The_tile_of_another_effect_stays_as_the_game_made_it()
    {
        var display = new StatusEffectDisplayUI { statusEffect = new ActiveStatusEffect() };

        TilePatches.CreatedPostfix(display);

        Assert.False(display.statusImage.enabled);
        Assert.False(display.positive);
    }

    [Fact]
    public void A_tile_with_no_effect_stays_as_the_game_made_it()
    {
        var display = new StatusEffectDisplayUI();

        TilePatches.CreatedPostfix(display);

        Assert.False(display.statusImage.enabled);
    }

    [Fact]
    public void Each_tile_create_writes_a_verbose_line()
    {
        Plugin.Verbose.Value = true;

        TilePatches.CreatedPostfix(new StatusEffectDisplayUI { statusEffect = TilePatches.Entry });

        Assert.Equal(new[] { "Road boost tile created" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void An_error_in_the_tile_create_gives_one_warning()
    {
        var display = new StatusEffectDisplayUI { statusEffect = TilePatches.Entry, statusImage = null };

        TilePatches.CreatedPostfix(display);

        Assert.Single(Plugin.Log.Warnings);
    }
}
