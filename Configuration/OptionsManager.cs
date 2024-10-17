using static ModernCamera.Configuration.OptionCategories;

namespace ModernCamera.Configuration;
internal static class OptionsManager
{
    public static Dictionary<string, OptionCategory> OptionCategories = [];
    public static OptionCategory AddCategory(string name)
    {
        if (!OptionCategories.ContainsKey(name)) OptionCategories.TryAdd(name, new(name));

        return OptionCategories[name];
    }
}