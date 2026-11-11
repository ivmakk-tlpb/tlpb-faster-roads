using HarmonyLib;

// Puts the static state of the fake game types back to a known start.
internal static class FakeState
{
    public static void Reset()
    {
        UnityEngine.Time.time = 0f;
        UnityEngine.Time.deltaTime = 0.01f;
        FasterRoads.Game.PlayerViews.Clear();
        FasterRoads.Patches.MovePatches.Reset();
        FasterRoads.Patches.StaminaPatches.Reset();
        FasterRoads.Patches.TilePatches.Reset();
        Harmony.Patched.Clear();
        Harmony.Unpatched.Clear();
        Harmony.ThrowFor = null;
    }
}
