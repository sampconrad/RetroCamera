using static RetroCamera.Configuration.SettingEvents;

namespace RetroCamera.Configuration;

[Serializable]
internal class Toggle(string name, string description, bool defaultvalue) : ChangeSetting<bool>(name, description, defaultvalue)
{
}
