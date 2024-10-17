using Stunlock.Localization;

namespace ModernCamera.Configuration;

public delegate void OnChange<T>(T value);
internal class OptionActions
{
    public class OptionAction<T>
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public virtual T Value { get; internal set; }
        public T DefaultValue { get; internal set; }

        public readonly LocalizationKey NameKey;
        public readonly LocalizationKey DescKey;
        public OptionAction(string name, string description, T defaultValue)
        {
            Name = name;
            Description = description;
            DefaultValue = defaultValue;
            Value = defaultValue;
            NameKey = LocalizationKeysManager.CreateKey(name);
            DescKey = LocalizationKeysManager.CreateKey(description);
        }

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
}