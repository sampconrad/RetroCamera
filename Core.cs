using BepInEx.Logging;
using ProjectM;
using ProjectM.Sequencer;
using ProjectM.UI;
using RetroCamera.Patches;
using Unity.Entities;

namespace RetroCamera;
internal class Core
{
    public static World Client;
    public static ZoomModifierSystem ZoomModifierSystem { get; internal set; }
    public static TopdownCameraSystem TopdownCameraSystem { get; internal set; }
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
        Systems.RetroCamera.ZoomModifierSystem = ZoomModifierSystem;

        TopdownCameraSystem = Client.GetExistingSystemManaged<TopdownCameraSystem>();

        PrefabCollectionSystem = Client.GetExistingSystemManaged<PrefabCollectionSystem>();
        Systems.RetroCamera.PrefabCollectionSystem = PrefabCollectionSystem;

        UIDataSystem = Client.GetExistingSystemManaged<UIDataSystem>();
        Systems.RetroCamera.UIDataSystem = UIDataSystem;

        CursorPositionSystem = Client.GetExistingSystemManaged<CursorPositionSystem>();
        Systems.RetroCamera.CursorPositionSystem = CursorPositionSystem;

        TopdownCameraSystemPatch.Initialize();

        HasInitialized = true;
    }
}