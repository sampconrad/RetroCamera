using BepInEx.Unity.IL2CPP.Hook;
using HarmonyLib;
using Il2CppInterop.Runtime;
using System.Reflection;

namespace RetroCamera.Utilities;
internal static class NativeDetour
{
    /*
    public static INativeDetour HandleInputDetour<T>(int methodToken, T to, out T original) where T : Delegate
    {
        Core.Log.LogInfo($"Detouring HandleInput...");
        IntPtr classPtr = Core.TopdownCameraSystem.Pointer;

        Core.Log.LogInfo($"Class pointer for TopdownCameraSystem retrieved...");
        IntPtr methodPtr = IL2CPP.GetIl2CppMethodByToken(classPtr, methodToken);

        if (methodPtr == IntPtr.Zero) throw new Exception($"Failed to create detour for TopdownCameraSystem with token {methodToken}...");
        else if (methodPtr != IntPtr.Zero) Core.Log.LogInfo($"Detouring TopdownCameraSystem {to.Method.Name} with token {methodToken} at {methodPtr.ToString("X")}...");
        
        return INativeDetour.CreateAndApply(methodPtr, to, out original);
    }
    public static INativeDetour UpdateCameraInputsDetour<T>(int methodToken, T to, out T original) where T : Delegate
    {
        Core.Log.LogInfo($"Detouring UpdateCameraInputs_Job...");
        IntPtr classPtr = Core.TopdownCameraSystem.Pointer;

        Core.Log.LogInfo($"Class pointer for TopdownCameraSystem retrieved...");
        IntPtr innerPtr = IL2CPP.GetIl2CppNestedType(classPtr, "UpdateCameraInputs_Job");
        IntPtr methodPtr = IL2CPP.GetIl2CppMethodByToken(innerPtr, methodToken);

        if (methodPtr == IntPtr.Zero) throw new Exception($"Failed to create detour for UpdateCameraInputs_Job with token {methodToken}...");
        else if (methodPtr != IntPtr.Zero) Core.Log.LogInfo($"Detouring UpdateCameraInputs_Job {to.Method.Name} with token {methodToken} at {methodPtr.ToString("X")}...");

        return INativeDetour.CreateAndApply(methodPtr, to, out original);
    }
    */
    public static INativeDetour Create<T>(Type type, string innerTypeName, string methodName, T to, out T original) where T : System.Delegate
    {
        return Create(GetInnerType(type, innerTypeName), methodName, to, out original);
    }
    public static INativeDetour Create<T>(Type type, string methodName, T to, out T original) where T : System.Delegate
    {
        return Create(type.GetMethod(methodName, AccessTools.all), to, out original);
    }
    static INativeDetour Create<T>(MethodInfo method, T to, out T original) where T : System.Delegate
    {
        var address = Il2CppMethodResolver.ResolveFromMethodInfo(method);
        return INativeDetour.CreateAndApply(address, to, out original);
    }
    static Type GetInnerType(Type type, string innerTypeName)
    {
        return type.GetNestedTypes().First(x => x.Name.Contains(innerTypeName));
    }
}
