using Stunlock.Localization;

namespace RetroCamera.Configuration;

[Serializable]
internal class OptionSettings(string name)
{
    public readonly LocalizationKey LocalizationKey = LocalizationManager.CreateKey(name);

    public static readonly Dictionary<string, Toggle> Toggles = [];
    public static readonly Dictionary<string, Slider> Sliders = [];
    public static readonly Dictionary<string, Dropdown> Dropdowns = [];

    public static readonly Dictionary<string, string> Dividers = [];
    public readonly List<string> SettingNames = [];
    public Toggle AddToggle(string name, string description, bool defaultValue)
    {
        Toggle setting = new(name, description, defaultValue);

        Toggles.TryAdd(name, setting);
        SettingNames.Add(setting.Name);

        return setting;
    }
    public Slider AddSlider(string name, string description, float minValue, float maxValue, float defaultValue, int decimals = default, float stepValue = default)
    {
        Slider setting = new(name, description, minValue, maxValue, defaultValue, decimals);

        Sliders.TryAdd(name, setting);
        SettingNames.Add(setting.Name);

        return setting;
    }
    public Dropdown AddDropdown(string name, string description, int defaultValue, string[] values)
    {
        Dropdown setting = new(name, description, defaultValue, values);

        Dropdowns.TryAdd(name, setting);
        SettingNames.Add(setting.Name);

        return setting;
    }
    public void AddDivider(string name)
    {
        string id = Guid.NewGuid().ToString();

        Dividers.TryAdd(id, name);
        SettingNames.Add(id);
    }
    public static bool TryGetToggle(string id, out Toggle setting)
    {
        if (!Toggles.ContainsKey(id))
        {
            setting = null;
            return false;
        }

        setting = Toggles[id];
        return true;
    }
    public static bool TryGetSlider(string id, out Slider setting)
    {
        if (!Sliders.ContainsKey(id))
        {
            setting = null;
            return false;
        }

        setting = Sliders[id];
        return true;
    }
    public static bool TryGetDropdown(string id, out Dropdown setting)
    {
        if (!Dropdowns.ContainsKey(id))
        {
            setting = null;
            return false;
        }

        setting = Dropdowns[id];
        return true;
    }
    public static bool TryGetDivider(string id, out string text)
    {
        if (!Dividers.ContainsKey(id))
        {
            text = null;
            return false;
        }

        text = Dividers[id];
        return true;
    }
}

