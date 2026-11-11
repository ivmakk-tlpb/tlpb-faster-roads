using BepInEx.Configuration;
using FasterRoads;
using FasterRoads.Patches;
using HarmonyLib;
using Xunit;

public class PluginTests : GameTestBase
{
    [Fact]
    public void The_guid_names_the_mod()
    {
        Assert.Equal("com.ivmakk.thelastplagueblight.fasterroads", Plugin.PluginGuid);
    }

    [Fact]
    public void The_speed_multiplier_is_1_33_by_default_within_1_to_3()
    {
        Assert.Equal(1.33f, Plugin.SpeedMultiplier.Value);
        var range = (AcceptableValueRange<float>)Plugin.SpeedMultiplier.Description.AcceptableValues;
        Assert.Equal(1f, range.MinValue);
        Assert.Equal(3f, range.MaxValue);
    }

    [Fact]
    public void The_stamina_drain_multiplier_is_0_5_by_default_within_0_to_1()
    {
        Assert.Equal(0.5f, Plugin.StaminaDrainMultiplier.Value);
        var range = (AcceptableValueRange<float>)Plugin.StaminaDrainMultiplier.Description.AcceptableValues;
        Assert.Equal(0f, range.MinValue);
        Assert.Equal(1f, range.MaxValue);
    }

    [Fact]
    public void The_road_icon_is_on_and_debug_lines_are_off_by_default()
    {
        Assert.True(Plugin.ShowRoadIcon.Value);
        Assert.False(Plugin.Verbose.Value);
    }

    [Fact]
    public void The_debug_helper_writes_only_while_verbose_is_on()
    {
        Plugin.Debug("hidden");
        Plugin.Verbose.Value = true;
        Plugin.Debug("shown");

        Assert.Equal(new[] { "shown" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void The_plugin_logs_that_it_loaded()
    {
        Assert.Contains("Faster Roads loaded.", Plugin.Log.Infos);
    }

    [Fact]
    public void The_hud_patches_are_in_the_boost_group()
    {
        Assert.Contains(typeof(HudPatches), Plugin.BoostGroup);
        Assert.Contains(typeof(HudPatches), Harmony.Patched);
        Assert.Empty(Plugin.Log.Warnings);
    }

    [Fact]
    public void The_speed_and_stamina_patches_are_in_the_boost_group()
    {
        Assert.Contains(typeof(MovePatches), Plugin.BoostGroup);
        Assert.Contains(typeof(StaminaPatches), Plugin.BoostGroup);
        Assert.Contains(typeof(MovePatches), Harmony.Patched);
        Assert.Contains(typeof(StaminaPatches), Harmony.Patched);
    }

    [Fact]
    public void A_missing_boost_target_turns_the_whole_mod_off_with_one_warning()
    {
        FakeState.Reset();
        Harmony.ThrowFor = typeof(StaminaPatches);
        new Plugin().Load();

        Assert.Equal(new[] { Plugin.PluginGuid }, Harmony.Unpatched);
        Assert.Equal(new[] { "Faster Roads is off: Target not found" }, Plugin.Log.Warnings);
    }

    [Fact]
    public void The_tile_patches_are_in_the_tile_group()
    {
        Assert.Contains(typeof(TilePatches), Plugin.TileGroup);
        Assert.Contains(typeof(TooltipPatches), Plugin.TileGroup);
        Assert.Contains(typeof(TooltipPatches), Harmony.Patched);
        Assert.DoesNotContain(typeof(TilePatches), Plugin.BoostGroup);
        Assert.Contains(typeof(TilePatches), Harmony.Patched);
    }

    [Fact]
    public void A_missing_tile_target_turns_only_the_tile_off_with_one_warning()
    {
        FakeState.Reset();
        Harmony.ThrowFor = typeof(TilePatches);
        new Plugin().Load();

        Assert.Equal(new[] { Plugin.PluginGuid }, Harmony.Unpatched);
        Assert.Equal(new[] { "Faster Roads shows no road boost tile: Target not found" }, Plugin.Log.Warnings);
        Assert.Contains(typeof(HudPatches), Harmony.Patched);
        Assert.Contains(typeof(MovePatches), Harmony.Patched);
        Assert.Contains(typeof(StaminaPatches), Harmony.Patched);
    }

    [Fact]
    public void A_repeated_error_gives_one_warning()
    {
        Plugin.WarnOnce("Part", new InvalidOperationException("broken"));
        Plugin.WarnOnce("Part", new InvalidOperationException("broken"));

        Assert.Single(Plugin.Log.Warnings);
    }
}
