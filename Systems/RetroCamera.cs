using ProjectM;
using ProjectM.Sequencer;
using ProjectM.UI;
using RetroCamera.Behaviours;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera.Systems;
public class RetroCamera : MonoBehaviour
{
    static GameObject _crosshairPrefab;
    static GameObject _crosshair;
    static CanvasScaler _canvasScaler;
    static Camera _gameCamera;

    public static ZoomModifierSystem _zoomModifierSystem;
    public static PrefabCollectionSystem _prefabCollectionSystem;
    public static UIDataSystem _uiDataSystem;
    public static CursorPositionSystem _cursorPositionSystem;

    public static GameObject _chatWindow;
    public static HUDChatWindow _hudChatWindow;

    public static bool _chatScroll;

    static bool _gameFocused = true;
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
        if (_zoomModifierSystem != null) _zoomModifierSystem.Enabled = !enabled;

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
        AddListeners();
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
    }
    static void GetOrCreateObjects()
    {
        if (_crosshairPrefab == null) BuildCrosshair();

        if (_gameCamera == null)
        {
            _gameCamera = CameraManager.GetCamera();
        }

        if (_chatWindow == null)
        {
            GameObject chatWindowObject = GameObject.Find("ChatWindow(Clone)");

            if (chatWindowObject != null)
            {
                _chatWindow = chatWindowObject;
                _hudChatWindow = _chatWindow.GetComponent<HUDChatWindow>();
            }
            else
            {
                Core.Log.LogWarning("ChatWindow(Clone) not found!");
            }
        }
    }
    void Update()
    {
        if (!Core._initialized) return;
        else if (!_gameFocused || !Settings.Enabled) return;

        if (_crosshairPrefab == null) BuildCrosshair();

        if (_gameCamera == null)
        {
            _gameCamera = CameraManager.GetCamera();
        }

        UpdateSystems();
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
    static void UpdateSystems()
    {
        if (_uiDataSystem == null || _prefabCollectionSystem == null) return;

        // should really just replace these with actual buff checks, interesting method of getting shapeshift/mounted status

        try
        {
            if (_uiDataSystem.UI.BuffBarParent != null)
            {
                _isShapeshifted = false;
                _shapeshiftName = "";

                foreach (BuffBarEntry.Data buff in _uiDataSystem.UI.BuffBarParent.BuffsSelectionGroup.Entries)
                {
                    if (_prefabCollectionSystem.PrefabGuidToNameDictionary.TryGetValue(buff.PrefabGUID, out string buffName))
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
                _isMounted = false;

                foreach (AbilityBarEntry abilityBarEntry in _uiDataSystem.UI.AbilityBar.Entries)
                {
                    if (_prefabCollectionSystem.PrefabGuidToNameDictionary.TryGetValue(abilityBarEntry.AbilityId, out string abilityName))
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

            // Locks the mouse to the center of the screen if the mouse should be locked or the camera rotate button is pressed
            if (_validGameplayInputState &&
               (_isMouseLocked || _gameplayInputState.IsInputPressed(ButtonInputAction.RotateCamera)) &&
               !IsMenuOpen)
            {
                if (_isActionMode || _isFirstPerson || Settings.CameraAimMode == CameraAimMode.Forward)
                {
                    CursorPosition cursorPosition = _cursorPositionSystem._CursorPosition;
                    float2 screenPosition = new((Screen.width / 2) + Settings.AimOffsetX, (Screen.height / 2) - Settings.AimOffsetY);

                    cursorPosition.ScreenPosition = screenPosition;
                    _cursorPositionSystem._CursorPosition = cursorPosition;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                // Set crosshair visibility based on the camera mode
                crosshairVisible = _isFirstPerson || (_isActionMode && Settings.ActionModeCrosshair);
                cursorVisible = false;
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
