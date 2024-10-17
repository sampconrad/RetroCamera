using Stunlock.Localization;

namespace ModernCamera.Configuration;

public delegate void OnChange<T>(T value);
internal class OptionActions<T>(string name, string description, T defaultValue)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public T DefaultValue { get; internal set; } = defaultValue;
    public virtual T Value { get; internal set; } = defaultValue;
    public LocalizationKey NameKey { get; } = LocalizationKeysManager.CreateKey(name);
    public LocalizationKey DescKey { get; } = LocalizationKeysManager.CreateKey(description);

    public event OnChange<T> OnChange = delegate { };
    public virtual void SetValue(T value)
    {
        Value = value;
        OnChange(value);
    }
    public void AddListener(OnChange<T> action)
    {
        OnChange += action;
    }
}