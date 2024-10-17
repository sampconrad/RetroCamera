using ModernCamera.Utilities;
using ProjectM;
using static ModernCamera.Configuration.Keybinding;

namespace ModernCamera.Configuration;
internal static class KeybindingManager
{
    public static readonly string KeybindingsPath = "KeybindEntries.json";
    public static readonly string KeybindingCategoriesPath = "KeybindCategories.json";

    public static readonly Dictionary<string, KeybindingCategory> KeybindingCategories = [];

    static readonly Dictionary<string, Keybinding> KeybindingsById = [];
    static readonly Dictionary<int, Keybinding> KeybindingsByGuid = [];
    static readonly Dictionary<ButtonInputAction, Keybinding> KeybindingsByFlags = [];
    public static readonly Dictionary<string, Keybindings> KeybindingValues = [];
    public struct KeybindingDescription
    {
        public string Id;
        public string Category;
        public string Name;
        public UnityEngine.KeyCode DefaultKeybinding;
    }
    public static Keybinding Register(KeybindingDescription description)
    {
        if (KeybindingsById.ContainsKey(description.Id)) throw new ArgumentException($"Keybinding with id {description.Id} already registered!");

        // Create the Keybinding instance
        Keybinding keybinding = new(description);

        // Store the keybinding
        KeybindingsById.TryAdd(description.Id, keybinding);
        KeybindingsByGuid.TryAdd(keybinding.AssetGuid, keybinding);
        KeybindingsByFlags.TryAdd(keybinding.InputFlag, keybinding);

        // Add to keybinding values
        if (!KeybindingValues.ContainsKey(description.Id))
        {
            Keybindings keybindings = new()
            {
                Id = description.Id,
                Primary = description.DefaultKeybinding,
                Secondary = UnityEngine.KeyCode.None
            };

            KeybindingValues.TryAdd(description.Id, keybindings);
        }

        return keybinding;
    }
    public static void Unregister(Keybinding keybinding)
    {
        if (!KeybindingsById.ContainsKey(keybinding.Description.Id))
        {
            throw new ArgumentException("There was no keybinding with id " + keybinding.Description.Id + " registered");
        }

        KeybindingsByFlags.Remove(keybinding.InputFlag);
        KeybindingsByGuid.Remove(keybinding.AssetGuid);
        KeybindingsById.Remove(keybinding.Description.Id);

        // Remove from keybinding values
        if (KeybindingValues.ContainsKey(keybinding.Description.Id))
        {
            KeybindingValues.Remove(keybinding.Description.Id);
        }
    }
    public static KeybindingCategory AddCategory(string name)
    {
        KeybindingCategory keybindingCategory = new(name);     
        KeybindingCategories[name] = keybindingCategory;

        return keybindingCategory;
    }
    public static Keybinding GetKeybinding(ButtonInputAction flag)
    {
        foreach (KeybindingCategory keybindingCategory in KeybindingCategories.Values)
        {
            if (keybindingCategory.HasKeybinding(flag))
            {
                return keybindingCategory.GetKeybinding(flag);
            }
        }

        return default;
    }
    public static void SaveKeybindings()
    {
        FileUtilities.WriteJson(KeybindingsPath, KeybindingValues);
    }
    public static void LoadKeybindings()
    {
        if (FileUtilities.Exists(KeybindingsPath))
        {
            Dictionary<string, Keybindings> keybindingValues = FileUtilities.ReadJson<Dictionary<string, Keybindings>>(KeybindingsPath);

            if (keybindingValues != null)
            {
                foreach (var keyValuePair in keybindingValues)
                {
                    KeybindingValues.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
    }
    public static void SaveKeybindingCategories()
    {
        FileUtilities.WriteJson(KeybindingCategoriesPath, KeybindingCategories);
    }
    public static void LoadKeybindingCategories()
    {
        if (FileUtilities.Exists(KeybindingCategoriesPath))
        {
            Dictionary<string, KeybindingCategory> keybindingCategories = FileUtilities.ReadJson<Dictionary<string, KeybindingCategory>>(KeybindingCategoriesPath);

            if (keybindingCategories != null)
            {
                foreach (var keyValuePair in keybindingCategories)
                {
                    KeybindingCategories.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
    }

}
