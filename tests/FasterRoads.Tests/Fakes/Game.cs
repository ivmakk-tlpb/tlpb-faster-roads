using UnityEngine;

// Fake game types: only the types and members that the mod source reaches.

// The game's component interfaces are classes in the interop. Here they are C# interfaces, so one view can
// have one of them, and FakeInterfaceCasts gives them the interop TryCast.
public interface IGameInterface
{
    IntPtr Pointer { get; }
}

public static class FakeInterfaceCasts
{
    public static T TryCast<T>(this IGameInterface value) where T : class => value as T;
}

public interface IEntityMovement : IGameInterface { }

public interface IStamina : IGameInterface { }

public interface IStatusEffects : IGameInterface { }

public enum SurfaceType
{
    NONE = 0,
    GRASS = 1,
    DIRT = 6,
    ROAD = 17,
    WET_ROAD = 18,
}

public static class ObjectLayer
{
    public static int terrainLayer = 8;
}

public class World : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase
{
    public SurfaceType Surface = SurfaceType.GRASS;
    public readonly List<Vector3> SurfaceLookups = new List<Vector3>();

    public SurfaceType GetSurfaceTypeAtTerrainPosition(Vector3 position)
    {
        SurfaceLookups.Add(position);
        return Surface;
    }
}

public class Entity : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase
{
    public int id;
    public World world;
    public readonly List<object> Components = new List<object>();

    public T GetCachedComponent<T>(bool write) where T : class
    {
        return Components.OfType<T>().FirstOrDefault();
    }
}

public class BaseView : MonoBehaviour
{
    public Entity entity;

    public Entity GetEntity() => entity;
}

public class SerializablePrimitiveField<T>
{
    public T Value;
}

public class EntityMovementView : BaseView, IEntityMovement
{
    public SerializablePrimitiveField<bool> movementEnabled = new SerializablePrimitiveField<bool> { Value = true };
    public bool climbingOver;
    public bool landing;
    public GameObject groundObject;
    public float movementSpeedChange;

    public void AddMovementSpeedChange(float multiplier)
    {
        movementSpeedChange = Math.Clamp(movementSpeedChange + multiplier, -0.8f, 5f);
    }
}

public class StaminaView : BaseView, IStamina
{
    public float staminaExertionRateChange;
    public bool ThrowOnAdd;

    public void AddStaminaExertionRateChange(float percentage)
    {
        if (ThrowOnAdd) throw new InvalidOperationException("broken");
        staminaExertionRateChange += percentage;
    }
}

public class StatusEffectsView : BaseView, IStatusEffects
{
    public void GetActiveStatusEffects(Il2CppSystem.Collections.Generic.ICollection<ActiveStatusEffect> statusEffectsResult) { }
}

public enum StatusEffectType
{
    NONE = 0,
    HEALTH_RATE = 1,
}

public enum IconType
{
    DECREASING = 8,
    INCREASING = 20,
    MOVEMENT = 30,
    PLUS = 36,
    STAMINA = 43,
}

public class StatusEffectDefinition { }

public class ActiveStatusEffect : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase
{
    public int itemId;
    public int substanceId;
    public StatusEffectDefinition statusEffectDefinition;
    public StatusEffectType statusEffectType;
    public float rate;
    public float hitpointsRecoveryRate;
    public float thirstReplenishmentRate;
    public float hungerReplenishmentRate;
    public float energyReplenishmentRate;
    public float diseaseRate;
    public float maxStaminaReductionAmount;
    public float diseaseHungerThirstIncreaseAmount;
    public float staminaRecoveryRateChange;
    public float staminaExertionRateChange;
    public float movementSpeedChange;
    public float diseaseSusceptibilityAmount;
    public float mosquitoRepellentAmount;
    public float actionSpeedChange;
    public float elapsedTime;
    public float duration;
    public Il2CppSystem.Collections.Generic.List<int> activeInjuryIds;
}

public class IUIManager
{
    public Sprite GetIconSprite(IconType type) => new Sprite { Icon = type };
}

public class UIComponent : MonoBehaviour
{
    public static IUIManager uiManager = new IUIManager();
}

public static class ColorUtil
{
    public static Color staminaColor = new Color(0.23f, 1f, 0.23f, 1f);
}

public class StatusEffectDisplayUI : UIComponent
{
    public UnityEngine.UI.RawImage itemImage = new UnityEngine.UI.RawImage { enabled = false };
    public UnityEngine.UI.Image statusImage = new UnityEngine.UI.Image { enabled = false };
    public UnityEngine.UI.Image indicatorImage = new UnityEngine.UI.Image { enabled = false };
    public ActiveStatusEffect statusEffect;
    public bool positive;

    public void showPositiveColors(IconType indicatorIconType)
    {
        positive = true;
        indicatorImage.sprite = uiManager.GetIconSprite(indicatorIconType);
    }
}

public class TooltipUI
{
    public readonly List<TooltipLine> Lines = new List<TooltipLine>();
}

public record TooltipLine(string Text, IconType? Icon, Color? Color, bool Bold);

public class TooltipLineUI { }

public class ShowTooltipEventHandler
{
    public bool ThrowOnLine;

    public TooltipLineUI addTooltipLine(TooltipUI tooltipUI, string text, bool bold)
    {
        if (ThrowOnLine) throw new InvalidOperationException("broken");
        tooltipUI.Lines.Add(new TooltipLine(text, null, null, bold));
        return new TooltipLineUI();
    }

    public TooltipLineUI addTooltipLine(TooltipUI tooltipUI, IconType iconType, string text, Color color)
    {
        tooltipUI.Lines.Add(new TooltipLine(text, iconType, color, false));
        return new TooltipLineUI();
    }

    public class __c__DisplayClass13_0
    {
        public string _addActiveStatusEffectTooltipLines_b__0(ActiveStatusEffect statusEffect) => "game title";
    }
}

public class LocalPlayer
{
    public Entity playerEntity;

    public Entity GetEntity() => playerEntity;
}

public class HudUI : MonoBehaviour
{
    public LocalPlayer player;
}
