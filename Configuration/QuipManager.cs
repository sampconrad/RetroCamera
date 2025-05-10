using RetroCamera.Utilities;
using Stunlock.Localization;

namespace RetroCamera.Configuration;
internal static class QuipManager
{
    public static IReadOnlyDictionary<int, CommandQuip> CommandQuips => _commandQuips;
    static readonly Dictionary<int, CommandQuip> _commandQuips = [];
    public readonly struct CommandQuip(string name, string command)
    {
        public readonly LocalizationKey NameKey = LocalizationManager.GetLocalizationKey(name);
        public string Name { get; init; } = name;
        public string Command { get; init; } = command;
    }
    public readonly struct Command
    {
        public string Name { get; init; }
        public string InputString { get; init; }
    }
    public static void TryLoadCommands()
    {
        var loaded = Persistence.LoadCommands();

        if (loaded != null)
        {
            foreach (var keyValuePair in loaded)
            {
                Command command = keyValuePair.Value;
                _commandQuips.TryAdd(keyValuePair.Key, new CommandQuip(command.Name, command.InputString));
            }
        }
    }
}
