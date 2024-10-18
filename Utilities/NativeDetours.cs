using BepInEx.Unity.IL2CPP.Hook;
using Il2CppInterop.Runtime;
using ProjectM;

namespace ModernCamera.Utilities;
internal static class NativeDetours
{
    public static INativeDetour Detour<T>(Type type, int methodToken, T to, out T original) where T : Delegate
    {
        IntPtr methodPtr = IL2CPP.GetIl2CppMethodByToken(Il2CppClassPointerStore<TopdownCameraSystem>.NativeClassPtr, methodToken);

        if (methodPtr == IntPtr.Zero) throw new Exception($"Failed to create detour for TopdownCameraSystem with token {methodToken} and type {type.FullName}...");
        else if (methodPtr != IntPtr.Zero) Core.Log.LogInfo($"Detouring {type.FullName} {to.Method.Name} with token {methodToken} at {methodPtr.ToString("X")}...");
        else Core.Log.LogInfo($"Detouring {type.FullName} method {to.Method.Name} with token {methodToken} at pointer {methodPtr.ToString("X")}.");
        
        return INativeDetour.CreateAndApply(methodPtr, to, out original);
    }
}
