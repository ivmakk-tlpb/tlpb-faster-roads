using System;
using FasterRoads.Game;
using HarmonyLib;

namespace FasterRoads.Patches
{
    // The road boost tile is one status effect entry for display only. It goes into the list of the player's status
    // effects only during the HUD update, so the save, which reads the same list at another time, never gets it.
    // The same entry comes back each frame, so the HUD keeps one tile for it and removes the tile when it is gone.
    internal static class TilePatches
    {
        private static ActiveStatusEffect entry;

        internal static bool InHud { get; private set; }

        internal static ActiveStatusEffect Entry => entry ??= CreateEntry();

        internal static void Reset()
        {
            InHud = false;
            entry = null;
        }

        internal static bool IsEntry(ActiveStatusEffect effect)
        {
            return effect != null && entry != null && effect.Pointer == entry.Pointer;
        }

        // No item, no substance, no definition, and no effect values, so the game adds no line or effect of its own.
        private static ActiveStatusEffect CreateEntry()
        {
            return new ActiveStatusEffect
            {
                statusEffectType = StatusEffectType.NONE,
                activeInjuryIds = new Il2CppSystem.Collections.Generic.List<int>(),
            };
        }

        [HarmonyPatch(typeof(HudUI), "updatePlayerDetailsHud")]
        [HarmonyPrefix]
        internal static void HudPrefix()
        {
            InHud = true;
        }

        // Runs also when the HUD update throws, so the flag never stays on.
        [HarmonyPatch(typeof(HudUI), "updatePlayerDetailsHud")]
        [HarmonyFinalizer]
        internal static void HudFinalizer()
        {
            InHud = false;
        }

        [HarmonyPatch(typeof(StatusEffectsView), nameof(StatusEffectsView.GetActiveStatusEffects))]
        [HarmonyPostfix]
        internal static void ActiveEffectsPostfix(StatusEffectsView __instance, Il2CppSystem.Collections.Generic.ICollection<ActiveStatusEffect> statusEffectsResult)
        {
            if (!InHud || __instance.Pointer != PlayerViews.StatusPointer || !RoadProbe.IsOn) return;
            try
            {
                if (!Boost.ShowTile(Plugin.SpeedMultiplier.Value, Plugin.StaminaDrainMultiplier.Value, Plugin.ShowRoadIcon.Value)) return;
                statusEffectsResult.Add(Entry);
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Road boost tile", e);
            }
        }

        // For type NONE the game turns the icon and the arrow off. The tile of the entry gets the game's movement
        // icon in the stamina green and the green up arrow, and the positive flag tints the tile green.
        [HarmonyPatch(typeof(StatusEffectDisplayUI), "Created")]
        [HarmonyPostfix]
        internal static void CreatedPostfix(StatusEffectDisplayUI __instance)
        {
            if (!IsEntry(__instance.statusEffect)) return;
            try
            {
                __instance.statusImage.enabled = true;
                __instance.indicatorImage.enabled = true;
                __instance.statusImage.sprite = UIComponent.uiManager.GetIconSprite(IconType.MOVEMENT);
                __instance.statusImage.color = ColorUtil.staminaColor;
                __instance.showPositiveColors(IconType.INCREASING);
                Plugin.Debug("Road boost tile created");
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Road boost tile", e);
            }
        }
    }
}
