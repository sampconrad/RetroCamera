using ProjectM;
using Stunlock.Localization;
using UnityEngine.InputSystem;
using static ModernCamera.Configuration.KeybindActions;
using static ModernCamera.Configuration.KeybindsManager;

namespace ModernCamera.Configuration;
internal class KeybindCategories
{
    public class KeybindCategory
    {
        public string Name;
        public InputActionMap InputActionMap;
        public LocalizationKey NameKey;
        public KeybindCategory(string name)
        {
            Name = name;
            InputActionMap = new(name);  // Initialize here, in the constructor
            NameKey = LocalizationKeysManager.CreateKey(name);
        }
        public KeybindMapping AddKeyBinding(string name, string category, string description, UnityEngine.KeyCode keyCode)
        {
            KeybindingDescription keybindingDescription = new()
            {
                Name = name,
                Category = category,
                Description = description,
                DefaultKeybind = keyCode
            };

            KeybindMapping keybinding = Register(keybindingDescription);
            return keybinding;
        }
    }
    public static KeybindMapping GetKeybind(string Id)
    {
        return KeybindsById.GetValueOrDefault(Id);
    }
    public static KeybindMapping GetKeybind(ButtonInputAction flag)
    {
        string id = IdentifiersByFlag.GetValueOrDefault(flag);
        return id == null ? default : GetKeybind(id);
    }
    public bool HasKeybinding(ButtonInputAction flag)
    {
        return KeybindsByFlag.ContainsKey(flag);
    }
}
