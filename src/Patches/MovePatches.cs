using System;
using FasterRoads.Game;
using HarmonyLib;
using UnityEngine;

namespace FasterRoads.Patches
{
    // Move multiplies the speed by 1 + movementSpeedChange and then sets that sum to 0, so a change added
    // before it applies once, together with the effects of the game. The prefix also runs the road check, so the
    // road state stays current while the player moves and the HUD does not update.
    [HarmonyPatch(typeof(EntityMovementView), "Move")]
    internal static class MovePatches
    {
        private static int loggedId = -1;

        internal static void Reset()
        {
            loggedId = -1;
        }

        [HarmonyPrefix]
        internal static void Prefix(EntityMovementView __instance)
        {
            if (__instance.Pointer != PlayerViews.MovementPointer) return;
            try
            {
                RoadProbe.Check(__instance, Time.time);
                if (!RoadProbe.IsOn) return;
                float change = Boost.SpeedChange(Plugin.SpeedMultiplier.Value);
                if (change == 0f) return;
                // Move returns before its reset on these calls, so a change added now would grow on each call.
                if (!__instance.movementEnabled.Value || __instance.climbingOver || __instance.landing) return;
                __instance.AddMovementSpeedChange(change);
                if (loggedId == PlayerViews.Id) return;
                loggedId = PlayerViews.Id;
                Plugin.Debug("Road speed boost applied to entity " + PlayerViews.Id);
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Speed boost", e);
            }
        }
    }
}
