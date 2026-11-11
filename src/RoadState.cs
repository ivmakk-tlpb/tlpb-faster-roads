namespace FasterRoads
{
    // Whether the road boost is on, from the road checks over time. A road result turns it on at once.
    // It turns off only when the grace time passed since the last road result, so the blended edge of a
    // road does not make it flicker.
    internal sealed class RoadState
    {
        internal const float GraceTime = 0.5f;
        internal const float CheckInterval = 0.1f;

        private bool checkedOnce;
        private float lastCheckTime;
        private float lastRoadTime;

        public bool IsOn { get; private set; }

        public bool ShouldCheck(float time)
        {
            return !checkedOnce || time - lastCheckTime >= CheckInterval - 0.0001f;
        }

        // Returns true when the result changed the state.
        public bool Update(float time, bool onRoad)
        {
            checkedOnce = true;
            lastCheckTime = time;
            if (onRoad) lastRoadTime = time;
            bool on = onRoad || (IsOn && time - lastRoadTime < GraceTime);
            if (on == IsOn) return false;
            IsOn = on;
            return true;
        }
    }
}
