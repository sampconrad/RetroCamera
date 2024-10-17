using Stunlock.Localization;

namespace ModernCamera.Configuration;

public delegate void OnChange<T>(T value);
internal class Option<T>(string name, string description, T defaultValue)
{
    public string Name { get; internal set; } = name;
    public string Description { get; internal set; } = description;
    public virtual T Value { get; internal set; } = defaultValue;
    public T DefaultValue { get; internal set; } = defaultValue;

    public event OnChange<T> OnChange = delegate { };
    public readonly LocalizationKey NameKey = LocalizationManager.CreateKey(name);
    public readonly LocalizationKey DescKey = LocalizationManager.CreateKey(description);
    public virtual void SetValue(T value)
    {
        Value = value;
        OnChange(Value);
    }
    public void AddListener(OnChange<T> action)
    {
        OnChange += action;
    }
}