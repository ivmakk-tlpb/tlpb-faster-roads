using System;
using FasterRoads.Game;
using HarmonyLib;
using UnityEngine;

namespace FasterRoads.Patches
{
    // StaminaView.Update multiplies the drain of moving by 1 + staminaExertionRateChange and sets that sum to 0
    // at its end, so the change does not reach the stamina cost of an action later in the frame. DrainPulse
    // gives the change of each frame.
    [HarmonyPatch(typeof(StaminaView), "Update")]
    internal static class StaminaPatches
    {
        private static int loggedId = -1;
        private static DrainPulse pulse = new DrainPulse();

        internal static void Reset()
        {
            loggedId = -1;
            pulse = new DrainPulse();
        }

        [HarmonyPrefix]
        internal static void Prefix(StaminaView __instance)
        {
            if (__instance.Pointer != PlayerViews.StaminaPointer || !RoadProbe.IsOn) return;
            try
            {
                float change = pulse.Next(Plugin.StaminaDrainMultiplier.Value, Time.deltaTime);
                if (change == 0f) return;
                __instance.AddStaminaExertionRateChange(change);
                if (loggedId == PlayerViews.Id) return;
                loggedId = PlayerViews.Id;
                Plugin.Debug("Road stamina boost applied to entity " + PlayerViews.Id);
            }
            catch (Exception e)
            {
                Plugin.WarnOnce("Stamina boost", e);
            }
        }
    }
}
