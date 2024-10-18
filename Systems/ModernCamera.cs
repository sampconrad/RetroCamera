using ModernCamera.Behaviours;
using ProjectM;
using ProjectM.Sequencer;
using ProjectM.UI;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using static ModernCamera.Utilities.CameraStateUtilities;

namespace ModernCamera.Systems;
public class ModernCamera : MonoBehaviour
{
    static GameObject CrosshairPrefab;
    static GameObject Crosshair;
    static CanvasScaler CanvasScaler;
    static Camera GameCamera;

    public static ZoomModifierSystem ZoomModifierSystem;
    public static PrefabCollectionSystem PrefabCollectionSystem;
    public static UIDataSystem UIDataSystem;
    public static CursorPositionSystem CursorPositionSystem;

    static bool GameFocused = true;
    public static void Enabled(bool enabled)
    {
        Settings.Enabled = enabled;
        UpdateEnabled(enabled);
    }
    public static void ActionMode(bool enabled)
    {
        IsMouseLocked = enabled;
        IsActionMode = enabled;
    }
    static void UpdateEnabled(bool enabled)
    {
        if (ZoomModifierSystem != null) ZoomModifierSystem.Enabled = !enabled;

        if (Crosshair != null) Crosshair.active = enabled && Settings.AlwaysShowCrosshair && !InBuildMode;

        if (!enabled)
        {
            Cursor.visible = true;
            ActionMode(false);
        }
    }
    static void UpdateFieldOfView(float fov)
    {
        if (GameCamera != null) GameCamera.fieldOfView = fov;
    }
    static void ToggleUI()
    {
        IsUIHidden = !IsUIHidden;
        DisableUISettings.SetHideHUD(IsUIHidden, Core.Client);
    }
    void Awake()
    {
        RegisterCameraBehaviour(new FirstPersonCameraBehaviour());
        RegisterCameraBehaviour(new ThirdPersonCameraBehaviour());

        Settings.AddEnabledListener(UpdateEnabled);
        Settings.AddFieldOfViewListener(UpdateFieldOfView);
        Settings.AddHideUIListener(ToggleUI);
    }
    void Update()
    {
        if (!Core.HasInitialized) return;
        else if (!GameFocused || !Settings.Enabled) return;

        if (CrosshairPrefab == null) BuildCrosshair();

        if (GameCamera == null)
        {
            GameObject cameraObject = GameObject.Find("Main_GameToolCamera(Clone)");

            if (cameraObject != null)
            {
                GameCamera = cameraObject.GetComponent<Camera>();
                UpdateFieldOfView(Settings.FieldOfView);
            }
        }

        UpdateSystems();
        UpdateCrosshair();
    }
    void OnApplicationFocus(bool hasFocus)
    {
        GameFocused = hasFocus;
    }
    static void BuildCrosshair()
    {
        try
        {
            CursorData cursorData = CursorController._CursorDatas.First(x => x.CursorType == CursorType.Game_Normal);
            if (cursorData == null) return;

            CrosshairPrefab = new("Crosshair");
            CrosshairPrefab.active = false;

            CrosshairPrefab.AddComponent<CanvasRenderer>();
            RectTransform rectTransform = CrosshairPrefab.AddComponent<RectTransform>();

            rectTransform.transform.SetSiblingIndex(1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(32, 32);
            rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            rectTransform.localPosition = new Vector3(0, 0, 0);

            Image image = CrosshairPrefab.AddComponent<Image>();
            image.sprite = Sprite.Create(cursorData.Texture, new Rect(0, 0, cursorData.Texture.width, cursorData.Texture.height), new Vector2(0.5f, 0.5f), 100f);

            CrosshairPrefab.active = false;
        }
        catch (Exception ex)
        {
            Core.Log.LogError(ex);
        }
    }
    static void UpdateSystems()
    {
        if (UIDataSystem == null || PrefabCollectionSystem == null) return;

        try
        {
            if (UIDataSystem.UI.BuffBarParent != null)
            {
                IsShapeshifted = false;
                ShapeshiftName = "";

                foreach (BuffBarEntry.Data buff in UIDataSystem.UI.BuffBarParent.BuffsSelectionGroup.Entries)
                {
                    if (PrefabCollectionSystem.PrefabGuidToNameDictionary.TryGetValue(buff.PrefabGUID, out string buffName))
                    {
                        IsShapeshifted = buffName.Contains("shapeshift", StringComparison.OrdinalIgnoreCase);

                        if (IsShapeshifted)
                        {
                            ShapeshiftName = buffName.Trim();
                            break;
                        }
                    }
                }
            }

            if (UIDataSystem.UI.AbilityBar != null)
            {
                IsMounted = false;

                foreach (AbilityBarEntry abilityBarEntry in UIDataSystem.UI.AbilityBar.Entries)
                {
                    if (PrefabCollectionSystem.PrefabGuidToNameDictionary.TryGetValue(abilityBarEntry.AbilityId, out string abilityName))
                    {
                        IsMounted = abilityName.Contains("mounted", StringComparison.OrdinalIgnoreCase);

                        if (IsMounted) break;
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

            if (Crosshair == null && CrosshairPrefab != null)
            {
                GameObject uiCanvas = GameObject.Find("HUDCanvas(Clone)/Canvas");

                CanvasScaler = uiCanvas.GetComponent<CanvasScaler>();
                Crosshair = Instantiate(CrosshairPrefab, uiCanvas.transform);
                Crosshair.active = true;
            }

            // Locks the mouse to the center of the screen if the mouse should be locked or the camera rotate button is pressed
            if (ValidGameplayInputState &&
               (IsMouseLocked || GameplayInputState.IsInputPressed(ButtonInputAction.RotateCamera)) &&
               !IsMenuOpen)
            {
                if (IsActionMode || IsFirstPerson || Settings.CameraAimMode == CameraAimMode.Forward)
                {
                    CursorPosition cursorPosition = CursorPositionSystem._CursorPosition;
                    float2 screenPosition = new((Screen.width / 2) + Settings.AimOffsetX, (Screen.height / 2) - Settings.AimOffsetY);

                    cursorPosition.ScreenPosition = screenPosition;
                    CursorPositionSystem._CursorPosition = cursorPosition;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                // Set crosshair visibility based on the camera mode
                crosshairVisible = IsFirstPerson || (IsActionMode && Settings.ActionModeCrosshair);
                cursorVisible = false;
            }

            if (Crosshair != null)
            {
                Crosshair.active = (crosshairVisible || Settings.AlwaysShowCrosshair) && !InBuildMode;

                if (IsFirstPerson)
                {
                    Crosshair.transform.localPosition = Vector3.zero;
                }
                else
                {
                    if (CanvasScaler != null)
                    {
                        Crosshair.transform.localPosition = new Vector3(
                            Settings.AimOffsetX * (CanvasScaler.referenceResolution.x / Screen.width),
                            Settings.AimOffsetY * (CanvasScaler.referenceResolution.y / Screen.height),
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
