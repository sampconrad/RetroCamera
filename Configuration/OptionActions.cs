using Stunlock.Localization;

namespace RetroCamera.Configuration;

public delegate void OnChangeHandler<T>(T value);
internal class OptionActions
{
    public class OptionAction<T>(string name, string description, T defaultValue)
    {
        public string Name { get; internal set; } = name;
        public string Description { get; internal set; } = description;
        public virtual T Value { get; internal set; } = defaultValue;
        public T DefaultValue { get; internal set; } = defaultValue;

        public readonly LocalizationKey NameKey = LocalizationKeyManager.CreateKey(name);
        public readonly LocalizationKey DescKey = LocalizationKeyManager.CreateKey(description);

        public event OnChangeHandler<T> OnChangeHandler = delegate { };
        public virtual void SetValue(T value)
        {
            Value = value;
            OnChangeHandler(value);
        }
        public void AddListener(OnChangeHandler<T> action)
        {
            OnChangeHandler += action;
        }
    }
}