namespace ModernCamera.Configuration;
internal static class OptionManager
{
    static readonly Dictionary<string, OptionCategory> OptionCategories = [];
    static readonly string OptionsPath = "OptionEntries.json";
    public static OptionCategory AddCategory(string name)
    {
        if (!OptionCategories.ContainsKey(name)) OptionCategories.TryAdd(name, new OptionCategory(name));

        return OptionCategories[name];
    }
    public static void SaveOptions()
    {
        Utilities.FileUtilities.WriteJson(OptionsPath, OptionCategories);
    }
    public static void LoadOptions()
    {
        if (Utilities.FileUtilities.Exists(OptionsPath))
        {
            Dictionary<string, OptionCategory> optionCategories = Utilities.FileUtilities.ReadJson<Dictionary<string, OptionCategory>>(OptionsPath);

            if (optionCategories != null)
            {
                foreach (var keyValuePair in optionCategories)
                {
                    OptionCategories.TryAdd(keyValuePair.Key, keyValuePair.Value);
                }
            }
        }
    }
}