// Fake Unity types: only the members that the mod source reaches.

namespace UnityEngine
{
    public struct Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }

    public struct Color
    {
        public float r, g, b, a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r; this.g = g; this.b = b; this.a = a;
        }
    }

    public static class Time
    {
        public static float time;
        public static float deltaTime;
    }

    public class Object : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase
    {
        // Unity's equality makes a destroyed object equal to null.
        public bool Destroyed;

        private static bool IsNull(Object o) => ReferenceEquals(o, null) || o.Destroyed;

        public static bool operator ==(Object a, Object b) => IsNull(a) ? IsNull(b) : ReferenceEquals(a, b);

        public static bool operator !=(Object a, Object b) => !(a == b);

        public override bool Equals(object other) => ReferenceEquals(this, other);

        public override int GetHashCode() => Pointer.GetHashCode();
    }

    public class Transform : Object
    {
        public Vector3 position;
    }

    public class GameObject : Object
    {
        public int layer;
        public readonly Transform transform = new Transform();
    }

    public class Component : Object
    {
        public readonly Transform transform = new Transform();
    }

    public class Behaviour : Component
    {
        public bool enabled = true;
    }

    public class MonoBehaviour : Behaviour { }

    public class Sprite : Object
    {
        // The icon that the fake UI manager made this sprite for.
        public global::IconType Icon;
    }
}

namespace UnityEngine.UI
{
    public class Image : Behaviour
    {
        public Sprite sprite;
        public Color color;
    }

    public class RawImage : Behaviour { }
}
