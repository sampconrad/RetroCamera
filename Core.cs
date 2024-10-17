using BepInEx.Logging;
using ProjectM;
using ProjectM.Sequencer;
using ProjectM.UI;
using Unity.Entities;

namespace ModernCamera;
internal class Core
{
    public static World Client;
    public static ZoomModifierSystem ZoomModifierSystem { get; internal set; }
    public static PrefabCollectionSystem PrefabCollectionSystem { get; internal set; }
    public static UIDataSystem UIDataSystem { get; internal set; }
    public static CursorPositionSystem CursorPositionSystem { get; internal set; }
    public static ManualLogSource Log => Plugin.LogInstance;

    public static bool HasInitialized = false;
    public static void Initialize(GameDataManager __instance)
    {
        if (HasInitialized) return;

        Client = __instance.World;

        ZoomModifierSystem = Client.GetExistingSystemManaged<ZoomModifierSystem>();
        ZoomModifierSystem.Enabled = false;
        Systems.ModernCamera.ZoomModifierSystem = ZoomModifierSystem;

        PrefabCollectionSystem = Client.GetExistingSystemManaged<PrefabCollectionSystem>();
        Systems.ModernCamera.PrefabCollectionSystem = PrefabCollectionSystem;

        UIDataSystem = Client.GetExistingSystemManaged<UIDataSystem>();
        Systems.ModernCamera.UIDataSystem = UIDataSystem;

        CursorPositionSystem = Client.GetExistingSystemManaged<CursorPositionSystem>();
        Systems.ModernCamera.CursorPositionSystem = CursorPositionSystem;

        HasInitialized = true;
    }
}