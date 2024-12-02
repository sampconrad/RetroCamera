using static RetroCamera.Configuration.OptionActions;

namespace RetroCamera.Configuration;
internal class ToggleOption : OptionAction<bool>
{
    public ToggleOption(string name, string description, bool defaultvalue) : base(name, description, defaultvalue)
    {

    }
}
