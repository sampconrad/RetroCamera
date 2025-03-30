using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using ProjectM;
using ProjectM.Physics;
using ProjectM.Scripting;
using ProjectM.Sequencer;
using ProjectM.UI;
using RetroCamera.Patches;
using RetroCamera.Utilities;
using System.Collections;
using Unity.Entities;
using UnityEngine;

namespace RetroCamera;
internal class Core
{
    public static World _client;
    public static EntityManager EntityManager => _client.EntityManager;
    public static ClientScriptMapper ClientScriptMapper { get; set; }
    public static ClientGameManager ClientGameManager { get; set; }
    public static ZoomModifierSystem ZoomModifierSystem { get; set; }
    public static TopdownCameraSystem TopdownCameraSystem { get; set; }
    public static PrefabCollectionSystem PrefabCollectionSystem { get; set; }
    public static UIDataSystem UIDataSystem { get; set; }
    public static CursorPositionSystem CursorPositionSystem { get; set; }
    public static ManualLogSource Log => Plugin.LogInstance;

    static MonoBehaviour _monoBehaviour;

    public static bool _initialized = false;
    public static void Initialize(GameDataManager __instance)
    {
        if (_initialized) return;

        _client = __instance.World;

        // ProjectM.Scripting.GameManager_Shared
        // ProjectM.Scripting.Game.SpellMods.TryApplyPrefabGUIDSpellMods

        ZoomModifierSystem = _client.GetExistingSystemManaged<ZoomModifierSystem>();
        ZoomModifierSystem.Enabled = false;
        Systems.RetroCamera._zoomModifierSystem = ZoomModifierSystem;

        TopdownCameraSystem = _client.GetExistingSystemManaged<TopdownCameraSystem>();

        PrefabCollectionSystem = _client.GetExistingSystemManaged<PrefabCollectionSystem>();
        Systems.RetroCamera._prefabCollectionSystem = PrefabCollectionSystem;

        UIDataSystem = _client.GetExistingSystemManaged<UIDataSystem>();
        Systems.RetroCamera._uiDataSystem = UIDataSystem;

        CursorPositionSystem = _client.GetExistingSystemManaged<CursorPositionSystem>();
        Systems.RetroCamera._cursorPositionSystem = CursorPositionSystem;

        ClientScriptMapper = _client.GetExistingSystemManaged<ClientScriptMapper>();
        ClientGameManager = ClientScriptMapper._ClientGameManager;

        TopdownCameraSystemPatch.Initialize();

        _initialized = true;
    }
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (_monoBehaviour == null)
        {
            var go = new GameObject("RetroCamera");
            _monoBehaviour = go.AddComponent<IgnorePhysicsDebugSystem>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }

        return _monoBehaviour.StartCoroutine(routine.WrapToIl2Cpp());
    }
    public static void ResetStates()
    {
        ClearSkies.ResetClearSkies();
        _initialized = false;
    }
}