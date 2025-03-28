using ProjectM;
using RetroCamera.Utilities;
using System.Text;
using UnityEngine;
using static RetroCamera.Configuration.Keybinding;

namespace RetroCamera.Configuration;

public delegate void KeybindHandler();

[Serializable]
internal static class KeybindManager
{
    public static IReadOnlyDictionary<string, Keybinding> KeybindCategoriesByName => _keybindCategoriesByName;
    static readonly Dictionary<string, Keybinding> _keybindCategoriesByName = [];
    public static IReadOnlyDictionary<string, Keybind> KeybindsByName => _keybindsByName;
    static readonly Dictionary<string, Keybind> _keybindsByName = [];
    public static IReadOnlyDictionary<Keybinding, List<Keybind>> KeybindsByKeybindCategory => _keybindsByKeybindCategory;
    static readonly Dictionary<Keybinding, List<Keybind>> _keybindsByKeybindCategory = [];
    public struct KeybindDescription
    {
        public string Name;
        public string Category;
        public string Description;
        public KeyCode DefaultKeybind;
    }
    public static Keybind Register(KeybindDescription description)
    {
        if (_keybindsByName.ContainsKey(description.Name)) throw new ArgumentException($"Keybinding with id {description.Name} already registered!");

        Keybind keybinding = new(description);

        _keybindsByName.TryAdd(description.Name, keybinding);

        if (_keybindCategoriesByName.TryGetValue(description.Category, out Keybinding category) && _keybindsByKeybindCategory.ContainsKey(category))
        {
            var keybinds = _keybindsByKeybindCategory[category];
            if (!keybinds.Contains(keybinding)) _keybindsByKeybindCategory[category].Add(keybinding);
        }

        return keybinding;
    }
    public static void Rebind(Keybind keybind, KeyCode keyCode)
    {
        keybind.Primary = keyCode;
        Persistence.SaveKeybinds();
    }
    public static Keybinding AddCategory(string name)
    {
        if (!_keybindCategoriesByName.ContainsKey(name))
        {
            Keybinding keybindCategory = new(name);
            _keybindCategoriesByName[name] = keybindCategory;
            _keybindsByKeybindCategory[keybindCategory] = [];
        }

        return _keybindCategoriesByName[name];
    }
    static ButtonInputAction ComputeInputFlag(string descriptionId)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(descriptionId);
        ulong num = Hash64(bytes);
        bool flag = false;

        do
        {
            foreach (ButtonInputAction buttonInputAction in Enum.GetValues<ButtonInputAction>())
            {
                if (num == (ulong)buttonInputAction)
                {
                    flag = true;
                    num--;
                }
            }
        } while (flag);

        return (ButtonInputAction)num;
    }
    static int ComputeAssetGuid(string descriptionId)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(descriptionId);
        return (int)Hash32(bytes);
    }
    static ulong Hash64(byte[] data)
    {
        ulong hash = 14695981039346656037UL;

        foreach (var b in data)
        {
            hash ^= b;
            hash *= 1099511628211UL;
        }

        return hash;
    }
    static uint Hash32(byte[] data)
    {
        uint hash = 2166136261U;

        foreach (var b in data)
        {
            hash ^= b;
            hash *= 16777619U;
        }

        return hash;
    }
    static readonly Dictionary<KeyCode, string> _keyLiterals = new()
    {
        // Punctuation & symbols
        { KeyCode.Space, " " },
        { KeyCode.BackQuote, "`" },
        { KeyCode.Minus, "-" },
        { KeyCode.Equals, "=" },
        { KeyCode.LeftBracket, "[" },
        { KeyCode.RightBracket, "]" },
        { KeyCode.Backslash, "\\" },
        { KeyCode.Semicolon, ";" },
        { KeyCode.Quote, "'" },
        { KeyCode.Comma, "," },
        { KeyCode.Period, "." },
        { KeyCode.Slash, "/" },

        // Top number keys
        { KeyCode.Alpha0, "0" },
        { KeyCode.Alpha1, "1" },
        { KeyCode.Alpha2, "2" },
        { KeyCode.Alpha3, "3" },
        { KeyCode.Alpha4, "4" },
        { KeyCode.Alpha5, "5" },
        { KeyCode.Alpha6, "6" },
        { KeyCode.Alpha7, "7" },
        { KeyCode.Alpha8, "8" },
        { KeyCode.Alpha9, "9" },

        // Letters
        { KeyCode.A, "A" },
        { KeyCode.B, "B" },
        { KeyCode.C, "C" },
        { KeyCode.D, "D" },
        { KeyCode.E, "E" },
        { KeyCode.F, "F" },
        { KeyCode.G, "G" },
        { KeyCode.H, "H" },
        { KeyCode.I, "I" },
        { KeyCode.J, "J" },
        { KeyCode.K, "K" },
        { KeyCode.L, "L" },
        { KeyCode.M, "M" },
        { KeyCode.N, "N" },
        { KeyCode.O, "O" },
        { KeyCode.P, "P" },
        { KeyCode.Q, "Q" },
        { KeyCode.R, "R" },
        { KeyCode.S, "S" },
        { KeyCode.T, "T" },
        { KeyCode.U, "U" },
        { KeyCode.V, "V" },
        { KeyCode.W, "W" },
        { KeyCode.X, "X" },
        { KeyCode.Y, "Y" },
        { KeyCode.Z, "Z" },

        // Arrow keys, etc.
        { KeyCode.UpArrow, "↑" },
        { KeyCode.DownArrow, "↓" },
        { KeyCode.LeftArrow, "←" },
        { KeyCode.RightArrow, "→" }
    };
    static string GetLiteral(KeyCode key)
    {
        return _keyLiterals.TryGetValue(key, out var literal) ? literal : key.ToString();
    }
}
