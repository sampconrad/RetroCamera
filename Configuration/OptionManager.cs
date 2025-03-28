namespace RetroCamera.Configuration;
internal static class OptionManager
{
    static readonly Dictionary<string, OptionSettings> _optionGroupSettingsByName = [];
    public static IReadOnlyDictionary<string, OptionSettings> OptionGroupSettingsByName => _optionGroupSettingsByName;
    public static OptionSettings AddCategory(string name)
    {
        if (!_optionGroupSettingsByName.ContainsKey(name)) _optionGroupSettingsByName.TryAdd(name, new(name));

        return _optionGroupSettingsByName[name];
    }
}