namespace ModernCamera.Configuration;
internal class DropdownOption(string name, string description, int defaultValue, string[] values) : Option<int>(name, description, defaultValue)
{
    public List<string> Values = [..values];
    public T GetEnumValue<T>()
    {
        return (T)Enum.Parse(typeof(T), Values[Value]);
    }
}
