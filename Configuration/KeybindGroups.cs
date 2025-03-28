using ProjectM;
using Stunlock.Localization;
using UnityEngine;
using UnityEngine.InputSystem;
using static RetroCamera.Configuration.KeybindManager;

namespace RetroCamera.Configuration;
internal class Keybinding(string name)
{
    public string Name = name;
    public InputActionMap InputActionMap = new(name);
    public LocalizationKey NameKey = LocalizationManager.CreateKey(name);
    public class Keybind(KeybindDescription description)
    {
        public KeybindDescription Description = description;

        public LocalizationKey NameKey = LocalizationManager.CreateKey(description.Name);
        public LocalizationKey DescriptionKey = LocalizationManager.CreateKey(description.Description);

        public KeyCode Primary = description.DefaultKeybind;
        public KeyCode Secondary = KeyCode.None;
        public string PrimaryName => GetLiteral(Primary);
        public string SecondaryName => GetLiteral(Secondary);

        public ButtonInputAction InputFlag = ComputeInputFlag(description.Name);
        public int AssetGuid = ComputeAssetGuid(description.Name);

        public event KeybindHandler KeyPressedHandler = delegate { };
        public event KeybindHandler KeyDownHandler = delegate { };
        public event KeybindHandler KeyUpHandler = delegate { };
        public void AddKeyPressedListener(KeybindHandler action) => KeyPressedHandler += action;
        public void AddKeyDownListener(KeybindHandler action) => KeyDownHandler += action;
        public void AddKeyUpListener(KeybindHandler action) => KeyUpHandler += action;
        public void OnKeyPressed() => KeyPressedHandler();
        public void OnKeyDown() => KeyDownHandler();
        public void OnKeyUp() => KeyUpHandler();
    }
    public static Keybind AddKeyBinding(string name, string category, string description, KeyCode keyCode)
    {
        KeybindDescription keybindingDescription = new()
        {
            Name = name,
            Category = category,
            Description = description,
            DefaultKeybind = keyCode
        };

        Keybind keybind = Register(keybindingDescription);
        return keybind;
    }
}


