using ProjectM;
using static RetroCamera.Configuration.KeybindCategories;
using static RetroCamera.Configuration.KeybindCategories.KeybindCategory;

namespace RetroCamera.Configuration;
internal static class KeybindsManager
{
    public static Dictionary<string, KeybindCategory> _keybindCategories = [];

    public static Dictionary<string, Keybind> _keybindsByName = [];
    public static readonly Dictionary<int, Keybind> KeybindsByGuid = [];

    public static readonly Dictionary<ButtonInputAction, Keybind> KeybindsByFlag = [];
    public static readonly Dictionary<ButtonInputAction, string> IdentifiersByFlag = [];
    public static readonly Dictionary<string, ButtonInputAction> FlagsByIndentifier = [];
    public struct KeybindingDescription
    {
        public string Name;
        public string Category;
        public string Description;
        public UnityEngine.KeyCode DefaultKeybind;
    }
    public static Keybind Register(KeybindingDescription description)
    {
        if (_keybindsByName.ContainsKey(description.Name)) throw new ArgumentException($"Keybinding with id {description.Name} already registered!");

        // Create the KeybindMapping instance
        Keybind keybinding = new(description);

        // Store the Id, Guid, and Flag paired with the KeybindMapping
        _keybindsByName.TryAdd(description.Name, keybinding);
        KeybindsByGuid.TryAdd(keybinding.AssetGuid, keybinding);
        KeybindsByFlag.TryAdd(keybinding.InputFlag, keybinding);
        IdentifiersByFlag.TryAdd(keybinding.InputFlag, description.Name);
        FlagsByIndentifier.TryAdd(description.Name, keybinding.InputFlag);

        return keybinding;
    }
    public static void Unregister(Keybind keybinding)
    {
        if (!_keybindsByName.ContainsKey(keybinding.Description.Name))
        {
            throw new ArgumentException("There was no keybinding with id " + keybinding.Description.Name + " registered");
        }

        KeybindsByFlag.Remove(keybinding.InputFlag);
        KeybindsByGuid.Remove(keybinding.AssetGuid);
        _keybindsByName.Remove(keybinding.Description.Name);
        IdentifiersByFlag.Remove(keybinding.InputFlag);
        FlagsByIndentifier.Remove(keybinding.Description.Name);
    }
    public static KeybindCategory AddCategory(string name)
    {
        if (!_keybindCategories.ContainsKey(name))
        {
            KeybindCategory keybindCategory = new(name);
            _keybindCategories[name] = keybindCategory;
        }

        return _keybindCategories[name];
    }
}
