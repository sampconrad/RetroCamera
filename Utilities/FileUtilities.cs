using BepInEx;
using System.Text.Json;

namespace ModernCamera.Utilities;
#nullable enable
public static class FileUtilities
{
    static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        IncludeFields = true
    };

    static readonly string DirectoryPath = Path.Join(Paths.ConfigPath, MyPluginInfo.PLUGIN_NAME);
    public static bool Exists(string fileName)
    {
        return File.Exists(Path.Join(DirectoryPath, fileName));
    }
    public static void WriteJson(string fileName, object? data)
    {
        try
        {
            if (!Directory.Exists(DirectoryPath)) Directory.CreateDirectory(DirectoryPath);

            string serialized = JsonSerializer.Serialize(data, JsonSerializerOptions);
            if (!serialized.IsNullOrWhiteSpace()) File.WriteAllText(Path.Join(DirectoryPath, fileName), serialized);
        }
        catch (Exception ex)
        {
            Core.Log.LogWarning($"Error saving {fileName}: {ex.Message}");
        }
    }
    public static T? ReadJson<T>(string fileName)
    {
        try
        {
            string content = File.ReadAllText(Path.Join(DirectoryPath, fileName));
            var deserialized = JsonSerializer.Deserialize<T>(content, JsonSerializerOptions);

            return deserialized;
        }
        catch (Exception ex)
        {
            Core.Log.LogWarning($"Error reading {fileName}: {ex.Message}");

            return default;
        }
    }
}