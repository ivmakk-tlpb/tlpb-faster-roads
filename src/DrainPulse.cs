namespace FasterRoads
{
    // The stamina change of each frame on a road. StaminaView.Update writes the stamina only when the change of a
    // frame is above 0.005, so a drain made smaller in each frame is lost at a high frame rate (at 0.5 above about
    // 100 fps). Each frame gets the full game drain (0) or no drain (-1) instead. The game drain of a frame is its
    // rate times the frame time, so the pulse counts frame time: the time of the frames with no drain is
    // 1 - multiplier of all the time, within half a frame, so the drain is the multiplier times the game drain
    // also when the frame time changes.
    internal sealed class DrainPulse
    {
        private float debt;

        public float Next(float multiplier, float frameTime)
        {
            if (frameTime <= 0f) return 0f;
            debt += (1f - Boost.Drain(multiplier)) * frameTime;
            if (debt < frameTime * 0.5f) return 0f;
            debt -= frameTime;
            return -1f;
        }
    }
}
