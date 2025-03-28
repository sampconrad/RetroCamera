using BepInEx;
using RetroCamera.Configuration;
using System.Text.Json;
using static RetroCamera.Configuration.Keybinding.Keybinding;

namespace RetroCamera.Utilities;
internal static class Persistence
{
    static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true
    };

    static readonly string _directoryPath = Path.Join(Paths.ConfigPath, MyPluginInfo.PLUGIN_NAME);

    const string KEYBINDS_KEY = "Keybinds";
    const string SETTINGS_KEY = "Settings";

    static readonly string _keybindsJson = Path.Combine(_directoryPath, $"{KEYBINDS_KEY}.json");
    static readonly string _settingsJson = Path.Combine(_directoryPath, $"{SETTINGS_KEY}.json");

    static readonly Dictionary<string, string> _filePaths = new()
    {
        {KEYBINDS_KEY, _keybindsJson },
        {SETTINGS_KEY, _settingsJson }
    };
    public static void SaveKeybinds() => SaveDictionary(KeybindManager.KeybindsByName, KEYBINDS_KEY);
    public static void SaveOptions() => SaveDictionary(OptionManager.OptionGroupSettingsByName, SETTINGS_KEY);
    public static Dictionary<string, Keybind> LoadKeybinds() => LoadDictionary<string, Keybind>(KEYBINDS_KEY);
    public static Dictionary<string, OptionSettings> LoadOptions() => LoadDictionary<string, OptionSettings>(SETTINGS_KEY);
    static Dictionary<T, U> LoadDictionary<T, U>(string fileKey)
    {
        if (!_filePaths.TryGetValue(fileKey, out string filePath)) return null;

        try
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
                return [];
            }

            string fileText = File.ReadAllText(filePath);
            return fileText.IsNullOrWhiteSpace()
                ? []
                : JsonSerializer.Deserialize<Dictionary<T, U>>(fileText, _jsonOptions);
        }
        catch (IOException ex)
        {
            Core.Log.LogWarning($"Failed to deserialize {fileKey} contents: {ex.Message}");
            return null;
        }
    }
    static void SaveDictionary<T, U>(IReadOnlyDictionary<T, U> fileData, string fileKey)
    {
        if (!_filePaths.TryGetValue(fileKey, out string filePath)) return;

        try
        {
            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            string fileText = JsonSerializer.Serialize(fileData, _jsonOptions);
            File.WriteAllText(filePath, fileText);
        }
        catch (IOException ex)
        {
            Core.Log.LogWarning($"Failed to serialize {fileKey} contents: {ex.Message}");
        }
    }

    /*
    public static void LoadKeybinds() => LoadDictionary(ref KeybindsManager.KeybindsByName, "KeybindMappings");
    public static void SaveKeybinds() => SaveDictionary<string, KeybindCategories.KeybindCategory.Keybind>(KeybindsManager.KeybindsByName, "KeybindMappings");
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
    */
}