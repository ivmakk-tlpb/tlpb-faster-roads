using FasterRoads;
using FasterRoads.Patches;
using UnityEngine;
using Xunit;

public class TooltipPatchesTests : GameTestBase
{
    private static readonly Color MarkerGreen = new Color(0.46f, 1f, 0.49f, 1f);
    private readonly ShowTooltipEventHandler handler = new ShowTooltipEventHandler();
    private readonly TooltipUI tooltip = new TooltipUI();

    private string Title(ActiveStatusEffect effect)
    {
        string result = "game title";
        TooltipPatches.TitlePostfix(effect, ref result);
        return result;
    }

    [Fact]
    public void The_title_of_the_entry_is_on_the_road()
    {
        Assert.Equal("On the Road", Title(TilePatches.Entry));
    }

    [Fact]
    public void The_title_of_another_effect_stays_as_the_game_made_it()
    {
        Assert.Equal("game title", Title(new ActiveStatusEffect()));
        Assert.Equal("game title", Title(null));
    }

    [Fact]
    public void The_tooltip_of_the_entry_has_the_description_and_both_boost_lines_in_green()
    {
        TooltipPatches.LinesPostfix(handler, tooltip, TilePatches.Entry);

        Assert.Equal(new[]
        {
            new TooltipLine("Roads make travel easier.", null, null, false),
            new TooltipLine("Movement speed is 33% faster", IconType.MOVEMENT, MarkerGreen, false),
            new TooltipLine("Stamina depletes 50% slower", IconType.STAMINA, MarkerGreen, false),
        }, tooltip.Lines);
    }

    [Fact]
    public void A_multiplier_of_one_hides_its_line()
    {
        Plugin.StaminaDrainMultiplier.Value = 1f;

        TooltipPatches.LinesPostfix(handler, tooltip, TilePatches.Entry);

        Assert.Equal(new[] { "Roads make travel easier.", "Movement speed is 33% faster" }, tooltip.Lines.Select(l => l.Text));
    }

    [Fact]
    public void The_tooltip_of_another_effect_gets_no_line()
    {
        TooltipPatches.LinesPostfix(handler, tooltip, new ActiveStatusEffect());
        TooltipPatches.LinesPostfix(handler, tooltip, null);

        Assert.Empty(tooltip.Lines);
    }

    [Fact]
    public void Each_tooltip_of_the_entry_writes_a_verbose_line()
    {
        Plugin.Verbose.Value = true;

        TooltipPatches.LinesPostfix(handler, tooltip, TilePatches.Entry);

        Assert.Equal(new[] { "Road boost tooltip shown" }, Plugin.Log.Debugs);
    }

    [Fact]
    public void An_error_in_the_tooltip_gives_one_warning()
    {
        handler.ThrowOnLine = true;

        TooltipPatches.LinesPostfix(handler, tooltip, TilePatches.Entry);
        TooltipPatches.LinesPostfix(handler, tooltip, TilePatches.Entry);

        Assert.Single(Plugin.Log.Warnings);
    }
}
