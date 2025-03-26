using static RetroCamera.Configuration.OptionCategories;

namespace RetroCamera.Configuration;
internal static class OptionsManager
{
    public static Dictionary<string, OptionCategory> _optionCategories = [];
    public static OptionCategory AddCategory(string name)
    {
        if (!_optionCategories.ContainsKey(name)) _optionCategories.TryAdd(name, new(name));

        return _optionCategories[name];
    }
}