using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace FasterRoads
{
    [BepInPlugin(PluginGuid, "Faster Roads", "1.0.0")]
    [BepInProcess("TheLastPlague.exe")]
    public sealed class Plugin : BasePlugin
    {
        public const string PluginGuid = "com.ivmakk.thelastplagueblight.fasterroads";

        internal static new ManualLogSource Log;
        internal static ConfigEntry<float> SpeedMultiplier;
        internal static ConfigEntry<float> StaminaDrainMultiplier;
        internal static ConfigEntry<bool> ShowRoadIcon;
        internal static ConfigEntry<bool> Verbose;

        // The road boost. A missing target turns the whole mod off.
        internal static readonly List<Type> BoostGroup = new List<Type>
        {
            typeof(Patches.HudPatches),
            typeof(Patches.MovePatches),
            typeof(Patches.StaminaPatches),
        };

        // The road boost tile and its tooltip. A missing target turns only the tile off.
        internal static readonly List<Type> TileGroup = new List<Type>
        {
            typeof(Patches.TilePatches),
            typeof(Patches.TooltipPatches),
        };

        private static readonly HashSet<string> warned = new HashSet<string>();

        public override void Load()
        {
            Log = base.Log;
            warned.Clear();
            SpeedMultiplier = Config.Bind(
                "General", "SpeedMultiplier", 1.33f,
                new ConfigDescription(
                    "Your movement speed on a road is the game speed times this value. 1.33 is 33% faster. 1.0 is as in the game.",
                    new AcceptableValueRange<float>(1f, 3f)));
            StaminaDrainMultiplier = Config.Bind(
                "General", "StaminaDrainMultiplier", 0.5f,
                new ConfigDescription(
                    "The stamina that moving takes on a road is the game drain times this value. 0.5 drains half, so you can run about twice as long. " +
                    "0.0 takes no stamina for moving on a road. 1.0 is as in the game. Stamina recovery and the stamina cost of actions do not change.",
                    new AcceptableValueRange<float>(0f, 1f)));
            ShowRoadIcon = Config.Bind(
                "General", "ShowRoadIcon", true,
                "Shows the road boost buff tile next to your status effects while you are on a road. With false, the boost still works, with no tile.");
            Verbose = Config.Bind(
                "General", "Verbose", false,
                "Writes extra diagnostic lines to the BepInEx log for troubleshooting. Keep off in normal play.");
            ApplyPatches();
            Log.LogInfo("Faster Roads loaded.");
        }

        // A game update can rename a target. The tile group goes first, so a failure there removes only the
        // tile patches. A failure in the boost group then removes every patch, and the game works as without the mod.
        private static void ApplyPatches()
        {
            var harmony = new Harmony(PluginGuid);
            try
            {
                foreach (var patches in TileGroup) harmony.PatchAll(patches);
            }
            catch (Exception e)
            {
                harmony.UnpatchSelf();
                Log.LogWarning("Faster Roads shows no road boost tile: " + e.Message);
            }
            try
            {
                foreach (var patches in BoostGroup) harmony.PatchAll(patches);
                Debug("All patches applied");
            }
            catch (Exception e)
            {
                harmony.UnpatchSelf();
                Log.LogWarning("Faster Roads is off: " + e.Message);
            }
        }

        // One warning for each distinct error, so a repeated error does not flood the log.
        internal static void WarnOnce(string where, Exception e)
        {
            string text = where + ": " + e.GetType().Name + ": " + e.Message;
            if (warned.Add(text)) Log.LogWarning(text + "\n" + e);
        }

        internal static void Debug(string line)
        {
            if (Verbose.Value) Log.LogDebug(line);
        }
    }
}
