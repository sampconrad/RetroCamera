using static RetroCamera.Configuration.OptionCategories;

namespace RetroCamera.Configuration;
internal static class OptionsManager
{
    public static Dictionary<string, OptionCategory> OptionCategories = [];
    public static OptionCategory AddCategory(string name)
    {
        if (!OptionCategories.ContainsKey(name)) OptionCategories.TryAdd(name, new(name));

        return OptionCategories[name];
    }
}