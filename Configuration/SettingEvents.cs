using Stunlock.Localization;

namespace RetroCamera.Configuration;

public delegate void SettingChangedHandler<T>(T value);
internal class SettingEvents
{
    public class ChangeSetting<T>(string name, string description, T defaultValue)
    {
        public string Name { get; internal set; } = name;
        public string Description { get; internal set; } = description;
        public virtual T Value { get; internal set; } = defaultValue;
        public T DefaultValue { get; internal set; } = defaultValue;

        public readonly LocalizationKey NameKey = LocalizationManager.CreateKey(name);
        public readonly LocalizationKey DescKey = LocalizationManager.CreateKey(description);

        public event SettingChangedHandler<T> OnChangeHandler = delegate { };
        public virtual void SetValue(T value)
        {
            Value = value;
            OnChangeHandler(value);
        }
        public void AddListener(SettingChangedHandler<T> action)
        {
            OnChangeHandler += action;
        }
    }
}