using System;

namespace FasterRoads.Game
{
    // The views of the local player's character, found from the HUD. A patch compares the pointer of its
    // view with these, which costs no read of a game object.
    internal static class PlayerViews
    {
        internal static int Id = -1;
        internal static EntityMovementView Movement;
        internal static IntPtr MovementPointer;
        internal static IntPtr StaminaPointer;
        internal static IntPtr StatusPointer;

        public static void Track(Entity entity)
        {
            if (entity == null)
            {
                if (Id != -1) Clear();
                return;
            }
            if (entity.id == Id && Movement != null) return;
            Clear();
            Id = entity.id;
            Movement = entity.GetCachedComponent<IEntityMovement>(false)?.TryCast<EntityMovementView>();
            MovementPointer = Movement != null ? Movement.Pointer : IntPtr.Zero;
            StaminaPointer = entity.GetCachedComponent<IStamina>(false)?.Pointer ?? IntPtr.Zero;
            StatusPointer = entity.GetCachedComponent<IStatusEffects>(false)?.Pointer ?? IntPtr.Zero;
        }

        public static void Clear()
        {
            Id = -1;
            Movement = null;
            MovementPointer = IntPtr.Zero;
            StaminaPointer = IntPtr.Zero;
            StatusPointer = IntPtr.Zero;
            RoadProbe.Reset();
        }
    }
}
