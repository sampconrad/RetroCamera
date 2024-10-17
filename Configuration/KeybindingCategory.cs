using ProjectM;
using Stunlock.Localization;
using UnityEngine.InputSystem;
using static ModernCamera.Configuration.KeybindingManager;

namespace ModernCamera.Configuration;
internal class KeybindingCategory
{
    public string Name { get; internal set; }

    public LocalizationKey NameKey;
    public InputActionMap InputActionMap;

    public readonly Dictionary<string, Keybinding> KeybindingMap = [];
    public static readonly Dictionary<ButtonInputAction, string> KeybindingFlags = [];
    public KeybindingCategory(string name)
    {
        Name = name;
        InputActionMap = new(name);
        NameKey = LocalizationManager.CreateKey(name);
    }
    public Keybinding AddKeyBinding(string name, string category, string description, UnityEngine.KeyCode keyCode)
    {
        // Create a KeybindingDescription
        KeybindingDescription keybindingDescription = new()
        {
            Id = name,
            Category = category,
            Name = description,
            DefaultKeybinding = keyCode
        };

        // Register the keybind using KeybindingManager
        Keybinding keybinding = Register(keybindingDescription);

        KeybindingMap.Add(name, keybinding);
        KeybindingFlags.Add(keybinding.InputFlag, name);

        return keybinding;
    }
    public Keybinding GetKeybinding(string id)
    {
        return KeybindingMap.GetValueOrDefault(id);
    }
    public Keybinding GetKeybinding(ButtonInputAction flag)
    {
        string id = KeybindingFlags.GetValueOrDefault(flag);
        return id == null ? default : GetKeybinding(id);
    }
    public bool HasKeybinding(ButtonInputAction flag)
    {
        return KeybindingFlags.ContainsKey(flag);
    }
}
