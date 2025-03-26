using Stunlock.Localization;
using UnityEngine;

namespace RetroCamera.Configuration;
internal class OptionCategories
{
    public class OptionCategory(string name)
    {
        public string Name { get; internal set; } = name;
        public readonly LocalizationKey LocalizationKey = LocalizationKeyManager.CreateKey(name);
        static readonly Dictionary<string, bool> _toggles = [];
        static readonly Dictionary<string, float> _sliders = [];
        static readonly Dictionary<string, string> _dropdowns = [];

        public static readonly Dictionary<string, ToggleOption> ToggleOptions = [];
        public static readonly Dictionary<string, SliderOption> SliderOptions = [];
        public static readonly Dictionary<string, DropdownOption> DropdownOptions = [];

        public static readonly Dictionary<string, string> Dividers = [];
        public readonly List<string> Options = [];
        public ToggleOption AddToggle(string name, string description, bool defaultValue)
        {
            ToggleOption option = new(name, description, defaultValue);

            if (_toggles.ContainsKey(name))
            {
                option.Value = _toggles[name];
            }

            ToggleOptions.TryAdd(name, option);
            Options.Add(option.Name);

            return option;
        }
        public SliderOption AddSlider(string name, string description, float minValue, float maxValue, float defaultValue, int decimals = default, float stepValue = default)
        {
            SliderOption option = new(name, description, minValue, maxValue, defaultValue, decimals);

            if (_sliders.ContainsKey(name))
            {
                option.Value = Mathf.Clamp(_sliders[name], minValue, maxValue);
            }

            SliderOptions.TryAdd(name, option);
            Options.Add(option.Name);

            return option;
        }
        public DropdownOption AddDropdown(string name, string description, int defaultValue, string[] values)
        {
            DropdownOption option = new(name, description, defaultValue, values);

            if (_dropdowns.ContainsKey(name))
            {
                option.Value = Mathf.Max(0, Array.IndexOf(values, _dropdowns[name]));
            }

            DropdownOptions.TryAdd(name, option);
            Options.Add(option.Name);

            return option;
        }
        public void AddDivider(string name)
        {
            string id = Guid.NewGuid().ToString();

            Dividers.TryAdd(id, name);
            Options.Add(id);
        }
        public static bool TryGetToggle(string id, out ToggleOption option)
        {
            if (!ToggleOptions.ContainsKey(id))
            {
                option = null;
                return false;
            }

            option = ToggleOptions[id];
            return true;
        }
        public static bool TryGetSlider(string id, out SliderOption option)
        {
            if (!SliderOptions.ContainsKey(id))
            {
                option = null;
                return false;
            }

            option = SliderOptions[id];
            return true;
        }
        public static bool TryGetDropdown(string id, out DropdownOption option)
        {
            if (!DropdownOptions.ContainsKey(id))
            {
                option = null;
                return false;
            }

            option = DropdownOptions[id];
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

    /*
    public ToggleOption GetToggle(string id)
    {
        return ToggleOptions.GetValueOrDefault(id);
    }
    public SliderOption GetSlider(string id)
    {
        return SliderOptions.GetValueOrDefault(id);
    }
    public DropdownOption GetDropdown(string id)
    {
        return DropdownOptions.GetValueOrDefault(id);
    }
    public bool HasToggle(string id)
    {
        return ToggleOptions.ContainsKey(id);
    }
    public bool HasSlider(string id)
    {
        return SliderOptions.ContainsKey(id);
    }
    public bool HasDropdown(string id)
    {
        return DropdownOptions.ContainsKey(id);
    }
    */
}
