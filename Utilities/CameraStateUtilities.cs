using ModernCamera.Behaviours;
using ProjectM;
using UnityEngine;

namespace ModernCamera.Utilities;
internal static class CameraStateUtilities
{
    public static bool IsUIHidden;
    public static bool IsFirstPerson;
    public static bool IsActionMode;
    public static bool IsMouseLocked;
    public static bool IsShapeshifted;
    public static bool IsMounted;
    public static bool InBuildMode;
    public static bool ValidGameplayInputState;

    public static BehaviourType CurrentBehaviourType = BehaviourType.Default;
    public static Dictionary<BehaviourType, CameraBehaviour> CameraBehaviours = [];
    public static InputState GameplayInputState;

    public static string ShapeshiftName;
    static int _menusOpen;
    public enum BehaviourType
    {
        Default,
        FirstPerson,
        ThirdPerson
    }
    public enum CameraAimMode
    {
        Default,
        Forward
    }
    public static bool IsMenuOpen
    {
        get { return _menusOpen > 0; }
        set { _menusOpen = value ? _menusOpen + 1 : Mathf.Max(0, _menusOpen - 1); }
    }
    public static CameraBehaviour CurrentCameraBehaviour
    {
        get
        {
            if (CameraBehaviours.ContainsKey(CurrentBehaviourType)) return CameraBehaviours[CurrentBehaviourType];
            else return null;
        }
    }
    public static void RegisterCameraBehaviour(CameraBehaviour behaviour)
    {
        Core.Log.LogInfo($"Registering camera behaviour {behaviour.BehaviourType}...");

        CameraBehaviours.Add(behaviour.BehaviourType, behaviour);
    }
    public static void Reset()
    {
        Core.Log.LogInfo("Resetting camera states...");

        IsUIHidden = false;
        IsFirstPerson = false;
        IsActionMode = false;
        IsMouseLocked = false;
        IsShapeshifted = false;
        IsMounted = false;
        InBuildMode = false;
        ShapeshiftName = "";
        ValidGameplayInputState = false;

        CurrentCameraBehaviour?.Deactivate();
        CurrentBehaviourType = BehaviourType.Default;
    }
}
