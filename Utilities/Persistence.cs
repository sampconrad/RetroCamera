using BepInEx;
using RetroCamera.Configuration;
using System.Text.Json;

namespace RetroCamera.Utilities;
internal static class Persistence
{
    static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true
    };

    static readonly string _directoryPath = Path.Join(Paths.ConfigPath, MyPluginInfo.PLUGIN_NAME);

    static readonly string _keybindMappingsJson = Path.Combine(_directoryPath, "KeybindMappings.json");
    static readonly string _optionCategoriesJson = Path.Combine(_directoryPath, "OptionCategories.json");

    static readonly Dictionary<string, string> FilePaths = new()
    {
        {"KeybindMappings", _keybindMappingsJson },
        {"OptionCategories", _optionCategoriesJson }
    };
    public static void LoadKeybinds() => LoadDictionary(ref KeybindsManager._keybindsByName, "KeybindMappings");
    public static void SaveKeybinds() => SaveDictionary(KeybindsManager._keybindsByName, "KeybindMappings");
    public static void LoadOptions() => LoadDictionary(ref OptionsManager._optionCategories, "OptionCategories");
    public static void SaveOptions() => SaveDictionary(OptionsManager._optionCategories, "OptionCategories");
    static void LoadDictionary<T, U>(ref Dictionary<T, U> fileData, string fileKey)
    {
        if (FilePaths.TryGetValue(fileKey, out string filePath))
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            try
            {
                string fileText = File.ReadAllText(filePath);

                if (fileText.IsNullOrWhiteSpace())
                {
                    fileData = [];
                }
                else
                {
                    fileData = JsonSerializer.Deserialize<Dictionary<T, U>>(fileText, _jsonOptions);
                }
            }
            catch (IOException ex)
            {
                Core.Log.LogWarning($"Failed to deserialize {fileKey} contents: {ex.Message}");
            }
        }
    }
    static void SaveDictionary<T, U>(Dictionary<T, U> fileData, string fileKey)
    {
        if (FilePaths.TryGetValue(fileKey, out string filePath))
        {
            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            try
            {
                string fileText = JsonSerializer.Serialize(fileData, _jsonOptions);

                if (fileText.IsNullOrWhiteSpace())
                {
                    fileData = [];
                }
                else
                {
                    File.WriteAllText(filePath, fileText);
                }
            }
            catch (IOException ex)
            {
                Core.Log.LogWarning($"Failed to serialize {fileKey} contents: {ex.Message}");
            }
        }
    }
}