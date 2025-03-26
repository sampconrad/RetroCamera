using ProjectM;
using Stunlock.Localization;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using static RetroCamera.Configuration.KeybindsManager;

namespace RetroCamera.Configuration;

public delegate void KeyEventHandler();
internal class KeybindCategories
{
    public class KeybindCategory(string name)
    {
        public string Name = name;
        public InputActionMap InputActionMap = new(name);
        public LocalizationKey NameKey = LocalizationKeyManager.CreateKey(name);
        // public LocalizationKey DescriptionKey = LocalizationKeyManager.CreateKey(name + "_Description");
        public class Keybind(KeybindingDescription description)
        {
            public KeybindingDescription Description = description;

            public KeyCode Primary = description.DefaultKeybind;
            public KeyCode Secondary = KeyCode.None;
            public string PrimaryName => Primary.ToString();
            public string SecondaryName => Secondary.ToString();

            public ButtonInputAction InputFlag = ComputeInputFlag(description.Name);
            public int AssetGuid = ComputeAssetGuid(description.Name);

            public event KeyEventHandler KeyPressedHandler = delegate { };
            public event KeyEventHandler KeyDownHandler = delegate { };
            public event KeyEventHandler KeyUpHandler = delegate { };

            public void AddKeyPressedListener(KeyEventHandler action) => KeyPressedHandler += action;
            public void AddKeyDownListener(KeyEventHandler action) => KeyDownHandler += action;
            public void AddKeyUpListener(KeyEventHandler action) => KeyUpHandler += action;
            public void OnKeyPressed() => KeyPressedHandler();
            public void OnKeyDown() => KeyDownHandler();
            public void OnKeyUp() => KeyUpHandler();
        }
        public static Keybind AddKeyBinding(string name, string category, string description, KeyCode keyCode)
        {
            KeybindingDescription keybindingDescription = new()
            {
                Name = name,
                Category = category,
                Description = description,
                DefaultKeybind = keyCode
            };

            Keybind keybinding = Register(keybindingDescription);
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
