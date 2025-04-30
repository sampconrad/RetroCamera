using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using ProjectM.Sequencer;
using ProjectM.UI;
using RetroCamera.Behaviours;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera.Systems;
public class RetroCamera : MonoBehaviour
{
    static EntityManager EntityManager => Core.EntityManager;
    static Entity _localCharacter;

    static GameObject _crosshairPrefab;
    static GameObject _crosshair;
    static CanvasScaler _canvasScaler;
    static Camera _gameCamera;
    static ZoomModifierSystem ZoomModifierSystem => Core.ZoomModifierSystem;

    static bool _gameFocused = true;
    static bool _listening = false;
    public static void Enabled(bool enabled)
    {
        Settings.Enabled = enabled;
        UpdateEnabled(enabled);
    }
    public static void ActionMode(bool enabled)
    {
        _isMouseLocked = enabled;
        _isActionMode = enabled;
    }
    static void UpdateEnabled(bool enabled)
    {
        if (ZoomModifierSystem != null) ZoomModifierSystem.Enabled = !enabled;

        if (_crosshair != null) _crosshair.active = enabled && Settings.AlwaysShowCrosshair && !_inBuildMode;

        if (!enabled)
        {
            Cursor.visible = true;
            ActionMode(false);
        }
    }
    static void UpdateFieldOfView(float fov)
    {
        if (_gameCamera != null) _gameCamera.fieldOfView = fov;
    }
    static void ToggleHUD()
    {
        _isUIHidden = !_isUIHidden;
        DisableUISettings.SetHideHUD(_isUIHidden, Core._client);
    }
    static void ToggleFog() => Utilities.ClearSkies.ToggleFog();
    void Awake()
    {
        Settings.Initialize();
        RegisterBehaviours();
        // GetOrCreateObjects();
    }
    static void RegisterBehaviours()
    {
        RegisterCameraBehaviour(new FirstPersonCameraBehaviour());
        RegisterCameraBehaviour(new ThirdPersonCameraBehaviour());
    }
    static void AddListeners()
    {
        Settings.AddEnabledListener(UpdateEnabled);
        Settings.AddFieldOfViewListener(UpdateFieldOfView);
        Settings.AddHideHUDListener(ToggleHUD);
        Settings.AddHideFogListener(ToggleFog);
        // Settings.AddCycleCameraListener(CycleCamera);
        Settings.AddCompleteTutorialListener(CompleteTutorial);
    }

    static GameObject _journalClaimButtonObject;
    static void CompleteTutorial()
    {
        if (_journalClaimButtonObject == null) _journalClaimButtonObject = GameObject.Find("HUDCanvas(Clone)/JournalCanvas/JournalParent(Clone)/Content/Layout/JournalEntry_Multi/ButtonParent/ClaimButton");

        if (_journalClaimButtonObject != null)
        {
            SimpleStunButton claimButton = _journalClaimButtonObject.GetComponent<SimpleStunButton>();
            claimButton?.Press();
        }
        else
        {
            Core.Log.LogWarning($"[RetroCamera] Journal claim button not found!");
        }
    }

    static readonly Dictionary<ProjectM.CameraType, Camera> _cameras = [];
    static Camera _camera;
    static CameraUser _cameraUser;

    static EntityQuery _cameraQuery;
    static void CycleCamera()
    {        
        try
        {
            if (_camera.Equals(null))
            {
                ComponentType[] cameraUserAllComponents =
                [
                    ComponentType.ReadOnly(Il2CppType.Of<CameraUser>())
                ];

                _cameraQuery = Core.BuildEntityQuery(EntityManager, cameraUserAllComponents, EntityQueryOptions.IncludeAll);

                _cameraUser = _cameraQuery.ToEntityArray(Allocator.Temp)[0].TryGetComponent(out CameraUser cameraUser) ? cameraUser : default;
                _camera = CameraUtilities.FindActiveCamera(EntityManager, _cameraQuery);

                if (_camera.Equals(null))
                {
                    Core.Log.LogWarning($"[RetroCamera] Camera is null!");
                }
                else
                {
                    Core.Log.LogWarning($"[RetroCamera] Camera found - {_camera.name}");
                }

                if (!_cameraUser.CameraEntity.Exists())
                {
                    Core.Log.LogWarning($"[RetroCamera] Camera entity on CameraUser is null!");
                }
                else
                {
                    Core.Log.LogWarning($"[RetroCamera] Camera entity on CameraUser exists!");
                    Core.LogEntity(Core._client, _cameraUser.CameraEntity);
                }
            }
        }
        catch (Exception ex)
        {
            Core.Log.LogWarning(ex);
        }
    }
    static void GetOrCreateObjects()
    {
        if (_crosshairPrefab == null) BuildCrosshair();

        if (_gameCamera == null)
        {
            _gameCamera = CameraManager.GetCamera();
        }
    }
    void Update()
    {
        if (!Core._initialized) return;
        else if (!_gameFocused || !Settings.Enabled) return;

        if (!_listening)
        {
            _listening = true;
            AddListeners();
        }

        if (_crosshairPrefab == null) BuildCrosshair();
        if (_gameCamera == null) _gameCamera = CameraManager.GetCamera();

        // UpdateSystems();
        UpdateCrosshair();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        _gameFocused = hasFocus;
    }
    static void BuildCrosshair()
    {
        try
        {
            CursorData cursorData = CursorController._CursorDatas.First(x => x.CursorType == CursorType.Game_Normal);
            if (cursorData == null) return;

            _crosshairPrefab = new("Crosshair")
            {
                active = false
            };

            _crosshairPrefab.AddComponent<CanvasRenderer>();
            RectTransform rectTransform = _crosshairPrefab.AddComponent<RectTransform>();

            rectTransform.transform.SetSiblingIndex(1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(32, 32);
            rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            rectTransform.localPosition = new Vector3(0, 0, 0);

            Image image = _crosshairPrefab.AddComponent<Image>();
            image.sprite = Sprite.Create(cursorData.Texture, new Rect(0, 0, cursorData.Texture.width, cursorData.Texture.height), new Vector2(0.5f, 0.5f), 100f);

            _crosshairPrefab.active = false;
        }
        catch (Exception ex)
        {
            Core.Log.LogError(ex);
        }
    }

    /* not sure if warranted tbh
    static void UpdateSystems()
    {
        if (_uiDataSystem == null || _prefabCollectionSystem == null) return;

        // should really just replace these with actual buff checks, interesting method of getting shapeshift/mounted status

        try
        {
            if (!_localCharacter.Exists())
            {
                _localCharacter = StunAnalytics.Client.TryGetLocalCharacter(EntityManager, out Entity result) ? result : default;
            }

            if (_uiDataSystem.UI.BuffBarParent != null)
            {
                _isShapeshifted = BuffUtility.HasBuff<Script_Buff_Shapeshift_DataShared>(EntityManager, _localCharacter);

                // _isShapeshifted = false;
                // _shapeshiftName = "";

                foreach (BuffBarEntry.Data buff in _uiDataSystem.UI.BuffBarParent.BuffsSelectionGroup.Entries)
                {
                    if (_prefabCollectionSystem.SpawnableNameToPrefabGuidDictionary.TryGetValue(buff.PrefabGUID, out string buffName))
                    {
                        _isShapeshifted = buffName.Contains("shapeshift", StringComparison.OrdinalIgnoreCase);

                        if (_isShapeshifted)
                        {
                            _shapeshiftName = buffName.Trim();
                            break;
                        }
                    }
                }
            }

            if (_uiDataSystem.UI.AbilityBar != null)
            {
                _isMounted = BuffUtility.HasBuff<MountBuff>(EntityManager, _localCharacter);

                foreach (AbilityBarEntry abilityBarEntry in _uiDataSystem.UI.AbilityBar.Entries)
                {
                    if (_prefabCollectionSystem.SpawnableNameToPrefabGuidDictionary.TryGetValue(abilityBarEntry.AbilityId, out string abilityName))
                    {
                        _isMounted = abilityName.Contains("mounted", StringComparison.OrdinalIgnoreCase);

                        if (_isMounted) break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Core.Log.LogError(ex);
        }
    }
    */
    static void UpdateCrosshair()
    {
        try
        {
            bool cursorVisible = true;
            bool crosshairVisible = false;

            if (_crosshair == null && _crosshairPrefab != null)
            {
                GameObject uiCanvas = GameObject.Find("HUDCanvas(Clone)/Canvas");

                if (uiCanvas == null) return;

                _canvasScaler = uiCanvas.GetComponent<CanvasScaler>();
                _crosshair = Instantiate(_crosshairPrefab, uiCanvas.transform);
                _crosshair.active = true;
            }

            if (_validGameplayInputState &&
               (_isMouseLocked || _gameplayInputState.IsInputPressed(ButtonInputAction.RotateCamera)) &&
               !IsMenuOpen)
            {
                
                // Set crosshair & cursor visibility based on the camera mode
                crosshairVisible = _isFirstPerson || (_isActionMode && Settings.ActionModeCrosshair);
                cursorVisible = _usingMouseWheel;
            }

            if (_crosshair != null)
            {
                _crosshair.active = (crosshairVisible || Settings.AlwaysShowCrosshair) && !_inBuildMode;

                if (_isFirstPerson)
                {
                    _crosshair.transform.localPosition = Vector3.zero;
                }
                else
                {
                    if (_canvasScaler != null)
                    {
                        _crosshair.transform.localPosition = new Vector3(
                            Settings.AimOffsetX * (_canvasScaler.referenceResolution.x / Screen.width),
                            Settings.AimOffsetY * (_canvasScaler.referenceResolution.y / Screen.height),
                            0
                        );
                    }
                }
            }

            Cursor.visible = cursorVisible;
        }
        catch (Exception ex)
        {
            Core.Log.LogError(ex);
        }
    }
}
