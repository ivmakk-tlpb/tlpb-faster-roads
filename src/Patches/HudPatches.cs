using System;
using FasterRoads.Game;
using HarmonyLib;
using UnityEngine;

namespace FasterRoads.Patches
{
    // The HUD updates the player details each frame, also while the player stands still. Its postfix finds
    // the local player and runs the road check, so the road state is current with no movement.
    [HarmonyPatch(typeof(HudUI), "updatePlayerDetailsHud")]
    internal static class HudPatches
    {
        [HarmonyPostfix]
        internal static void Postfix(HudUI __instance)
        {
            try
            {
                PlayerViews.Track(__instance.player?.GetEntity());
                RoadProbe.Check(PlayerViews.Movement, Time.time);
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Road check", e);
            }
        }
    }
}
