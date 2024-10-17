using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Runtime;

namespace ModernCamera.Utilities;
internal static class InteropUtilities
{
    public static INativeDetour Detour<T>(Type type, int methodToken, T to, out T original) where T : Delegate
    {
        IntPtr classPtr = Il2CppClassPointerStore.GetNativeClassPointer(type);
        IntPtr methodPtr = IL2CPP.GetIl2CppMethodByToken(classPtr, methodToken);

        if (methodPtr == IntPtr.Zero) throw new Exception($"Failed to create detour for {to.Method.Name} with token {methodToken} in type {type.FullName}...");
        else if (methodPtr != IntPtr.Zero) Core.Log.LogInfo($"Detouring {type.FullName} {to.Method.Name} with token {methodToken} at {methodPtr.ToString("X")}...");

        return INativeDetour.CreateAndApply(methodPtr, to, out original);
    }
}
