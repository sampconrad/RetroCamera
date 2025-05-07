using ProjectM;
using ProjectM.Sequencer;
using ProjectM.UI;
using RetroCamera.Behaviours;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera.Systems;
public class RetroCamera : MonoBehaviour
{
    static EntityManager EntityManager => Core.EntityManager;
    static ZoomModifierSystem ZoomModifierSystem => Core.ZoomModifierSystem;
    static ActionWheelSystem ActionWheelSystem => Core.ActionWheelSystem;

    static GameObject _crosshairPrefab;
    static GameObject _crosshair;
    static CanvasScaler _canvasScaler;
    public static Camera GameCamera => _gameCamera;
    static Camera _gameCamera;

    static bool _gameFocused = true;
    static bool _listening = false;
    static bool HideCharacterInfoPanel => Settings.HideCharacterInfoPanel;
    static GameObject _characterInfoPanel;
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
        if (!Settings.Enabled) return;

        _isUIHidden = !_isUIHidden;
        DisableUISettings.SetHideHUD(_isUIHidden, Core._client);
    }
    static void ToggleFog()
    {
        if (!Settings.Enabled) return;

        Utilities.ClearSkies.ToggleFog();
    }
    void Awake()
    {
        Settings.Initialize();
        RegisterBehaviours();
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
        // Settings.AddSocialWheelPressedListener(SocialWheelKeyPressed);
        // Settings.AddSocialWheelUpListener(SocialWheelKeyUp);
        Settings.AddCompleteTutorialListener(CompleteTutorial);
    }

    static GameObject _journalClaimButtonObject;
    static void CompleteTutorial()
    {
        if (!Settings.Enabled) return;
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
    public static bool SocialWheelActive => _socialWheelActive;
    static bool _socialWheelActive = false;
    public static ActionWheel SocialWheel => _socialWheel;
    static ActionWheel _socialWheel;
    public static bool _shouldActivateWheel = false;

    static Entity _rootPrefabCollection;
    static bool _socialWheelInitialized = false;

    static Entity _freeCameraPrefab;
    static Entity _topDownCameraPrefab;
    static Entity _orbitCameraPrefab;
    static void SocialWheelKeyPressed()
    {
        /*
        if (!_rootPrefabCollection.Exists()) ActionWheelSystem._RootPrefabCollectionAccessor.TryGetSingletonEntity(out _rootPrefabCollection);
        
        if (!_socialWheelInitialized && _rootPrefabCollection.TryGetComponent(out RootPrefabCollection rootPrefabCollection) 
            && rootPrefabCollection.GeneralGameplayCollectionPrefab.TryGetComponent(out GeneralGameplayCollection generalGameplayCollection))
        {
            ActionWheelSystem.InitializeSocialWheel(true, generalGameplayCollection);
            _socialWheelInitialized = true;

            try
            {
                var socialWheelData = ActionWheelSystem._SocialWheelDataList;
                var socialWheelShortcuts = ActionWheelSystem._SocialWheelShortcutList;

                Core.Log.LogWarning($"[SocialWheelKeyPressed] Social Wheel - {socialWheelData.Count}, {socialWheelShortcuts.Count}");
                int index = 0;

                foreach (var data in socialWheelData)
                {
                    Core.Log.LogWarning($"[RetroCamera] ({index++}) Data - ActionType: {data.Type}, Disabled: {data.Disabled}, Unlocked: {data.Unlocked}, PrefabGUID: {data.PrefabGUID}");
                }

                index = 0;

                foreach (var shortcut in socialWheelShortcuts)
                {
                    Core.Log.LogWarning($"[RetroCamera] ({index++}) Shortcut - ActionType: {shortcut.Type}, Disabled: {shortcut.Disabled}, Unlocked: {shortcut.Unlocked}, PrefabGUID: {shortcut.PrefabGUID}");
                }

                if (rootPrefabCollection.FreeCameraPrefab.Exists())
                {
                    FreeCameraPrefab = rootPrefabCollection.FreeCameraPrefab;
                    Core.LogEntity(Core._client, rootPrefabCollection.FreeCameraPrefab);
                }

                if (rootPrefabCollection.TopDownCameraPrefab.Exists())
                {
                    TopDownCameraPrefab = rootPrefabCollection.TopDownCameraPrefab;
                    Core.LogEntity(Core._client, rootPrefabCollection.TopDownCameraPrefab);
                }

                if (rootPrefabCollection.OrbitCameraPrefab.Exists())
                {
                    OrbitCameraPrefab = rootPrefabCollection.OrbitCameraPrefab;
                    Core.LogEntity(Core._client, rootPrefabCollection.OrbitCameraPrefab);
                }
            }
            catch (Exception ex)
            {
                Core.Log.LogError(ex);
            }
        }

        if (_socialWheel == null)
        {
            _socialWheel = ActionWheelSystem._SocialWheel;
        }
        */

        _socialWheelActive = true;
        _shouldActivateWheel = true;
        _socialWheel.gameObject.SetActive(_socialWheelActive);
    }
    static void SocialWheelKeyUp()
    {
        if (_socialWheelActive && !_shouldActivateWheel)
        {
            ActionWheelSystem.HideCurrentWheel();
            _socialWheelActive = false;
            _socialWheel.gameObject.SetActive(_socialWheelActive);
        }
    }

    static readonly Dictionary<ProjectM.CameraType, Camera> _cameras = [];
    static Camera _camera;
    static CameraUser _cameraUser;

    static EntityQuery _cameraQuery;
    static void CycleCamera()
    {      
        /*
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
        */
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
        // else if (!Settings.Enabled) return;

        if (!_listening)
        {
            _listening = true;
            AddListeners();
        }

        if (_crosshairPrefab == null) BuildCrosshair();
        if (_gameCamera == null) _gameCamera = CameraManager.GetCamera();

        if (_characterInfoPanel == null)
        {
            GameObject characterInfoPanelCanvas = GameObject.Find("HUDCanvas(Clone)/TargetInfoPanelCanvas");
            _characterInfoPanel = characterInfoPanelCanvas?.transform.GetChild(0).gameObject;

            if (_characterInfoPanel == null)
            {
                // _characterInfoPanel = characterInfoPanelCanvas?.transform.GetChild(0).gameObject;
                Core.Log.LogWarning($"[RetroCamera] CharacterInfoPanel (0) not found!");
            }
            else
            {
                Core.Log.LogWarning($"[RetroCamera] CharacterInfoPanel (0) found!");
            }
        }

        /*
        if (!_rootPrefabCollection.Exists() || _socialWheel == null)
        {
            ActionWheelSystem?._RootPrefabCollectionAccessor.TryGetSingletonEntity(out _rootPrefabCollection);

            if (!_socialWheelInitialized && _rootPrefabCollection.TryGetComponent(out RootPrefabCollection rootPrefabCollection)
                && rootPrefabCollection.GeneralGameplayCollectionPrefab.TryGetComponent(out GeneralGameplayCollection generalGameplayCollection))
            {
                ActionWheelSystem.InitializeSocialWheel(true, generalGameplayCollection);
                _socialWheelInitialized = true;

                try
                {
                    var socialWheelData = ActionWheelSystem._SocialWheelDataList;
                    var socialWheelShortcuts = ActionWheelSystem._SocialWheelShortcutList;

                    if (rootPrefabCollection.FreeCameraPrefab.Exists())
                    {
                        _freeCameraPrefab = rootPrefabCollection.FreeCameraPrefab;
                        // Core.LogEntity(Core._client, rootPrefabCollection.FreeCameraPrefab);
                    }

                    if (rootPrefabCollection.TopDownCameraPrefab.Exists())
                    {
                        _topDownCameraPrefab = rootPrefabCollection.TopDownCameraPrefab;
                        // Core.LogEntity(Core._client, rootPrefabCollection.TopDownCameraPrefab);
                    }

                    if (rootPrefabCollection.OrbitCameraPrefab.Exists())
                    {
                        _orbitCameraPrefab = rootPrefabCollection.OrbitCameraPrefab;
                        // Core.LogEntity(Core._client, rootPrefabCollection.OrbitCameraPrefab);
                    }
                }
                catch (Exception ex)
                {
                    Core.Log.LogError(ex);
                }
            }

            _socialWheel = ActionWheelSystem?._SocialWheel;
        }
        */

        // UpdateSystems();
        UpdateCrosshair();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        // _gameFocused = hasFocus;
        if (hasFocus) IsMenuOpen = false;
    }
    public static void RebuildCrosshair()
    {
        if (_crosshair != null)
        {
            GameObject.Destroy(_crosshair);
            _crosshair = null;
        }
        if (_crosshairPrefab != null)
        {
            GameObject.Destroy(_crosshairPrefab);
            _crosshairPrefab = null;
        }
        BuildCrosshair();
    }

    static Texture2D CreateDotTexture()
    {
        int size = 24;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colors = new Color[size * size];

        float radius = size / 6f;
        float center = size / 2f;
        float radiusSquared = radius * radius;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float distanceSquared = dx * dx + dy * dy;

                if (distanceSquared <= radiusSquared)
                {
                    float alpha = (1f - (distanceSquared / radiusSquared)) * 0.7f;
                    colors[y * size + x] = new Color(0.8f, 0.8f, 0.8f, alpha);
                }
                else
                {
                    colors[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }

    static void BuildCrosshair()
    {
        try
        {
            Texture2D crosshairTexture;
            if (Settings.UseDotCrosshair)
            {
                crosshairTexture = CreateDotTexture();
            }
            else
            {
                CursorData cursorData = CursorController._CursorDatas.First(x => x.CursorType == CursorType.Game_Normal);
                if (cursorData == null) return;
                crosshairTexture = cursorData.Texture;
            }

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
            rectTransform.sizeDelta = new Vector2(24, 24);
            rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            rectTransform.localPosition = new Vector3(0, 0, 0);

            Image image = _crosshairPrefab.AddComponent<Image>();
            image.sprite = Sprite.Create(crosshairTexture, new Rect(0, 0, crosshairTexture.width, crosshairTexture.height), new Vector2(0.5f, 0.5f), 100f);
            image.preserveAspect = true;

            _crosshairPrefab.active = false;

            // If we already have a crosshair instance, recreate it with the new prefab
            if (_crosshair != null)
            {
                GameObject uiCanvas = GameObject.Find("HUDCanvas(Clone)/Canvas");
                if (uiCanvas != null)
                {
                    GameObject.Destroy(_crosshair);
                    _crosshair = Instantiate(_crosshairPrefab, uiCanvas.transform);
                    _crosshair.active = true;
                }
            }
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

            bool shouldHandle = _validGameplayInputState &&
               (_isMouseLocked || _gameplayInputState.IsInputPressed(ButtonInputAction.RotateCamera));

            if (shouldHandle && !IsMenuOpen)
            {
                if (_isActionMode && HideCharacterInfoPanel)
                {
                    _characterInfoPanel.SetActive(false);
                }
                else
                {
                    _characterInfoPanel.SetActive(true);
                }

                crosshairVisible = _isFirstPerson || (_isActionMode && Settings.ActionModeCrosshair);
                cursorVisible = _usingMouseWheel;
            }
            // hide mouse when rotating in build mode?
            else if (shouldHandle && _inBuildMode)
            {
                crosshairVisible = Settings.AlwaysShowCrosshair;
                cursorVisible = false;
            }

            if (_crosshair != null)
            {
                // _crosshair.active = (crosshairVisible || Settings.AlwaysShowCrosshair) && !_inBuildMode;
                _crosshair.active = crosshairVisible || Settings.AlwaysShowCrosshair;

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
