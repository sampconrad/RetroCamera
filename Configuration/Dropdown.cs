using static RetroCamera.Configuration.SettingEvents;

namespace RetroCamera.Configuration;

[Serializable]
internal class Dropdown : ChangeSetting<int>
{
    public List<string> Values;
    public Dropdown(string name, string description, int defaultValue, string[] values) : base(name, description, defaultValue)
    {
        Values = [];

        foreach (var v in values)
        {
            Values.Add(v);
        }
    }
    public T GetEnumValue<T>()
    {
        return (T)Enum.Parse(typeof(T), Values[Value]);
    }
}
