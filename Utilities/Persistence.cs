using BepInEx;
using ModernCamera.Configuration;
using System.Text.Json;

namespace ModernCamera.Utilities;
internal static class Persistence
{
    static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true
    };

    static readonly string DirectoryPath = Path.Join(Paths.ConfigPath, MyPluginInfo.PLUGIN_NAME);

    static readonly string KeybindMappingsJson = Path.Combine(DirectoryPath, "KeybindMappings.json");
    static readonly string OptionCategoriesJson = Path.Combine(DirectoryPath, "OptionCategories.json");

    static readonly Dictionary<string, string> FilePaths = new()
    {
        {"KeybindMappings", KeybindMappingsJson },
        {"OptionCategories", OptionCategoriesJson }
    };
    public static void LoadKeybinds() => LoadDictionary(ref KeybindsManager.KeybindsById, "KeybindMappings");
    public static void SaveKeybinds() => SaveDictionary(KeybindsManager.KeybindsById, "KeybindMappings");
    public static void LoadOptions() => LoadDictionary(ref OptionsManager.OptionCategories, "OptionCategories");
    public static void SaveOptions() => SaveDictionary(OptionsManager.OptionCategories, "OptionCategories");
    static void LoadDictionary<T, U>(ref Dictionary<T, U> fileData, string fileKey)
    {
        if (FilePaths.TryGetValue(fileKey, out string filePath))
        {
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
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
                    fileData = JsonSerializer.Deserialize<Dictionary<T, U>>(fileText, JsonOptions);
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
            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            try
            {
                string fileText = JsonSerializer.Serialize(fileData, JsonOptions);

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