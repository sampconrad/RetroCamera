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

    public static bool _initialized = false;
    public static void Initialize(GameDataManager __instance)
    {
        if (_initialized) return;

        Client = __instance.World;

        ZoomModifierSystem = Client.GetExistingSystemManaged<ZoomModifierSystem>();
        ZoomModifierSystem.Enabled = false;
        Systems.RetroCamera._zoomModifierSystem = ZoomModifierSystem;

        TopdownCameraSystem = Client.GetExistingSystemManaged<TopdownCameraSystem>();

        PrefabCollectionSystem = Client.GetExistingSystemManaged<PrefabCollectionSystem>();
        Systems.RetroCamera._prefabCollectionSystem = PrefabCollectionSystem;

        UIDataSystem = Client.GetExistingSystemManaged<UIDataSystem>();
        Systems.RetroCamera._uiDataSystem = UIDataSystem;

        CursorPositionSystem = Client.GetExistingSystemManaged<CursorPositionSystem>();
        Systems.RetroCamera._cursorPositionSystem = CursorPositionSystem;

        TopdownCameraSystemPatch.Initialize();

        _initialized = true;
    }
}