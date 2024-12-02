using ProjectM;
using Stunlock.Localization;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static RetroCamera.Configuration.KeybindsManager;

namespace RetroCamera.Configuration;

public delegate void KeyEvent();
internal class KeybindCategories
{
    public class KeybindCategory
    {
        public string Name;
        public InputActionMap InputActionMap;
        public LocalizationKey NameKey;
        public KeybindCategory(string name)
        {
            Name = name;
            InputActionMap = new(name);  // Initialize here, in the constructor
            NameKey = LocalizationKeysManager.CreateKey(name);
        }
        // Nested KeybindMapping class
        public class KeybindMapping
        {
            public KeybindingDescription Description;
            public KeyCode Primary;
            public KeyCode Secondary;
            public ButtonInputAction InputFlag;
            public int AssetGuid;

            public event KeyEvent KeyPressed = delegate { };
            public event KeyEvent KeyDown = delegate { };
            public event KeyEvent KeyUp = delegate { };

            public KeybindMapping(KeybindingDescription description)
            {
                Description = description;
                Primary = description.DefaultKeybind;
                Secondary = KeyCode.None;
                InputFlag = ComputeInputFlag(description.Name);
                AssetGuid = ComputeAssetGuid(description.Name);
            }

            public void AddKeyPressedListener(KeyEvent action) => KeyPressed += action;
            public void AddKeyDownListener(KeyEvent action) => KeyDown += action;
            public void AddKeyUpListener(KeyEvent action) => KeyUp += action;
            public void OnKeyPressed() => KeyPressed();
            public void OnKeyDown() => KeyDown();
            public void OnKeyUp() => KeyUp();
        }
        public KeybindMapping AddKeyBinding(string name, string category, string description, KeyCode keyCode)
        {
            KeybindingDescription keybindingDescription = new()
            {
                Name = name,
                Category = category,
                Description = description,
                DefaultKeybind = keyCode
            };

            KeybindMapping keybinding = Register(keybindingDescription);
            return keybinding;
        }
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
}
