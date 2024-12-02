using static RetroCamera.Configuration.OptionActions;

namespace RetroCamera.Configuration;
internal class DropdownOption : OptionAction<int>
{
    public List<string> Values;
    public DropdownOption(string name, string description, int defaultValue, string[] values) : base(name, description, defaultValue)
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
