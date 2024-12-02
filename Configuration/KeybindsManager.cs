using ProjectM;
using static RetroCamera.Configuration.KeybindCategories;
using static RetroCamera.Configuration.KeybindCategories.KeybindCategory;

namespace RetroCamera.Configuration;
internal static class KeybindsManager
{
    public static Dictionary<string, KeybindCategory> KeybindCategories = [];

    public static Dictionary<string, KeybindMapping> KeybindsById = [];
    public static readonly Dictionary<int, KeybindMapping> KeybindsByGuid = [];

    public static readonly Dictionary<ButtonInputAction, KeybindMapping> KeybindsByFlag = [];
    public static readonly Dictionary<ButtonInputAction, string> IdentifiersByFlag = [];
    public static readonly Dictionary<string, ButtonInputAction> FlagsByIndentifier = [];
    public struct KeybindingDescription
    {
        public string Name;
        public string Category;
        public string Description;
        public UnityEngine.KeyCode DefaultKeybind;
    }
    public static KeybindMapping Register(KeybindingDescription description)
    {
        if (KeybindsById.ContainsKey(description.Name)) throw new ArgumentException($"Keybinding with id {description.Name} already registered!");

        // Create the KeybindMapping instance
        KeybindMapping keybinding = new(description);

        // Store the Id, Guid, and Flag paired with the KeybindMapping
        KeybindsById.TryAdd(description.Name, keybinding);
        KeybindsByGuid.TryAdd(keybinding.AssetGuid, keybinding);
        KeybindsByFlag.TryAdd(keybinding.InputFlag, keybinding);
        IdentifiersByFlag.TryAdd(keybinding.InputFlag, description.Name);
        FlagsByIndentifier.TryAdd(description.Name, keybinding.InputFlag);

        return keybinding;
    }
    public static void Unregister(KeybindMapping keybinding)
    {
        if (!KeybindsById.ContainsKey(keybinding.Description.Name))
        {
            throw new ArgumentException("There was no keybinding with id " + keybinding.Description.Name + " registered");
        }

        KeybindsByFlag.Remove(keybinding.InputFlag);
        KeybindsByGuid.Remove(keybinding.AssetGuid);
        KeybindsById.Remove(keybinding.Description.Name);
        IdentifiersByFlag.Remove(keybinding.InputFlag);
        FlagsByIndentifier.Remove(keybinding.Description.Name);
    }
    public static KeybindCategory AddCategory(string name)
    {
        if (!KeybindCategories.ContainsKey(name))
        {
            KeybindCategory keybindCategory = new(name);
            KeybindCategories[name] = keybindCategory;
        }

        return KeybindCategories[name];
    }
}
