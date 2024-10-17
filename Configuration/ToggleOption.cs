using static ModernCamera.Configuration.OptionActions;

namespace ModernCamera.Configuration;
internal class ToggleOption : OptionAction<bool>
{
    public ToggleOption(string name, string description, bool defaultvalue) : base(name, description, defaultvalue)
    {

    }
}
