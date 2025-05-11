using ProjectM;
using Stunlock.Localization;
using System.Text.Json.Serialization;
using UnityEngine;

namespace RetroCamera.Configuration;

[Serializable]
internal class Keybinding
{
    public string Name;
    public string Description;

    public KeyCode Primary = KeyCode.None;
    public string PrimaryName => KeybindsManager.GetLiteral(Primary);

    public delegate void KeyHandler();

    public event KeyHandler OnKeyPressed = delegate { };
    public event KeyHandler OnKeyDown = delegate { };
    public event KeyHandler OnKeyUp = delegate { };

    [JsonIgnore]
    public LocalizationKey NameKey;

    [JsonIgnore]
    public LocalizationKey DescriptionKey;

    [JsonIgnore]
    public ButtonInputAction InputFlag;

    [JsonIgnore]
    public int AssetGuid;
    public Keybinding() { }
    public Keybinding(string name, string description, KeyCode defaultKey)
    {
        Name = name;
        Description = description;
        Primary = defaultKey;
        NameKey = LocalizationManager.GetLocalizationKey(name);
        DescriptionKey = LocalizationManager.GetLocalizationKey(description);
        InputFlag = KeybindsManager.ComputeInputFlag(name);
        AssetGuid = KeybindsManager.ComputeAssetGuid(name);
    }
    public void AddKeyPressedListener(KeyHandler action) => OnKeyPressed += action;
    public void AddKeyDownListener(KeyHandler action) => OnKeyDown += action;
    public void AddKeyUpListener(KeyHandler action) => OnKeyUp += action;
    public void KeyPressed() => OnKeyPressed();
    public void KeyDown() => OnKeyDown();
    public void KeyUp() => OnKeyUp();
    public void ApplySaved(Keybinding keybind)
    {
        if (keybind == null) return;

        Primary = keybind.Primary;
    }
}
