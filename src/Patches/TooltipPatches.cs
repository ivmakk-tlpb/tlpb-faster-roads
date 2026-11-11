using System;
using HarmonyLib;
using UnityEngine;

namespace FasterRoads.Patches
{
    // The tooltip of the road boost tile. The entry has no definition and no effect values, so the game adds no
    // line of its own, and the mod adds its description and one line for each boost.
    internal static class TooltipPatches
    {
        // The marker green of StatusEffectDisplayUI.showPositiveColors, rgb(117, 255, 125).
        private static readonly Color MarkerGreen = new Color(0.46f, 1f, 0.49f, 1f);

        // The title line reads this lambda again each frame. For an entry with duration 0 it has GetTitle inlined,
        // so the patch is on the lambda.
        [HarmonyPatch(typeof(ShowTooltipEventHandler.__c__DisplayClass13_0),
            nameof(ShowTooltipEventHandler.__c__DisplayClass13_0._addActiveStatusEffectTooltipLines_b__0))]
        [HarmonyPostfix]
        internal static void TitlePostfix(ActiveStatusEffect statusEffect, ref string __result)
        {
            if (TilePatches.IsEntry(statusEffect)) __result = RoadText.Title;
        }

        [HarmonyPatch(typeof(ShowTooltipEventHandler), "addActiveStatusEffectTooltipLines")]
        [HarmonyPostfix]
        internal static void LinesPostfix(ShowTooltipEventHandler __instance, TooltipUI tooltipUI, ActiveStatusEffect activeStatusEffect)
        {
            if (!TilePatches.IsEntry(activeStatusEffect)) return;
            try
            {
                __instance.addTooltipLine(tooltipUI, RoadText.Description, false);
                AddBoostLine(__instance, tooltipUI, IconType.MOVEMENT, RoadText.SpeedLine(Plugin.SpeedMultiplier.Value));
                AddBoostLine(__instance, tooltipUI, IconType.STAMINA, RoadText.DrainLine(Plugin.StaminaDrainMultiplier.Value));
                Plugin.Debug("Road boost tooltip shown");
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Road boost tooltip", e);
            }
        }

        private static void AddBoostLine(ShowTooltipEventHandler handler, TooltipUI tooltipUI, IconType icon, string line)
        {
            if (line != null) handler.addTooltipLine(tooltipUI, icon, line, MarkerGreen);
        }
    }
}
