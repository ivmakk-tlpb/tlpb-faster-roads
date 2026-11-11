// Fake IL2CPP interop types and the fake IL2CPP collections.

namespace Il2CppInterop.Runtime.InteropTypes
{
    public class Il2CppObjectBase
    {
        private static long nextPointer = 0x10000;

        public IntPtr Pointer { get; } = new IntPtr(nextPointer += 8);

        public T TryCast<T>() where T : Il2CppObjectBase => this as T;
    }
}

namespace Il2CppSystem.Collections.Generic
{
    public class ICollection<T> : Il2CppInterop.Runtime.InteropTypes.Il2CppObjectBase
    {
        public readonly System.Collections.Generic.List<T> Items = new System.Collections.Generic.List<T>();

        public void Add(T item) { Items.Add(item); }

        public int Count => Items.Count;
    }

    public class List<T> : ICollection<T> { }
}
