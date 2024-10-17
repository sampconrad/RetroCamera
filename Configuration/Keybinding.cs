using UnityEngine;
using ProjectM;
using Stunlock.Localization;
using System.Text;
using UnityEngine.InputSystem;
using static ModernCamera.Configuration.KeybindingManager;

namespace ModernCamera.Configuration;

public delegate void KeyEvent();
internal class Keybinding
{
    public KeybindingDescription Description { get; }
    public KeyCode Primary => KeybindingValues[Description.Id].Primary;
    public KeyCode Secondary => KeybindingValues[Description.Id].Secondary;
    public bool IsPressed => Input.GetKey(Primary) || Input.GetKey(Secondary);
    public bool IsDown => Input.GetKeyDown(Primary) || Input.GetKeyDown(Secondary);
    public bool IsUp => Input.GetKeyUp(Primary) || Input.GetKeyUp(Secondary);

    public event KeyEvent KeyPressed = delegate { };
    public event KeyEvent KeyDown = delegate { };
    public event KeyEvent KeyUp = delegate { };
    public ButtonInputAction InputFlag { get; private set; }
    public LocalizationKey NameKey;
    public InputAction InputAction;
    public string DefaultPrimary { get; set; }
    public string DefaultSecondary { get; set; }

    public int AssetGuid;
    public Keybinding(KeybindingDescription description)
    {
        Description = description;

        ComputeInputFlag();
        ComputeAssetGuid();
    }

    // Constructor
    public class Keybindings
    {
        public string Id { get; set; }
        public KeyCode Primary { get; set; }
        public KeyCode Secondary { get; set; }
    }

    // Hash computation for InputFlag and AssetGuid
    void ComputeInputFlag()
    {
        byte[] bytes = Encoding.UTF8.GetBytes(Description.Id);
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

        InputFlag = (ButtonInputAction)num;
    }
    void ComputeAssetGuid()
    {
        byte[] bytes = Encoding.UTF8.GetBytes(Description.Id);
        AssetGuid = (int)Hash32(bytes);
    }

    // Hashing utility methods
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

    // Event invocation
    public void OnKeyPressed() => KeyPressed();
    public void OnKeyDown() => KeyDown();
    public void OnKeyUp() => KeyUp();

    // Add event listeners
    public void AddKeyPressedListener(KeyEvent action) => KeyPressed += action;
    public void AddKeyDownListener(KeyEvent action) => KeyDown += action;
    public void AddKeyUpListener(KeyEvent action) => KeyUp += action;

    // Method for rebinding
    public void StartRebinding(bool isPrimary, Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> onComplete, Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> onCancel)
    {
        int bindingIndex = isPrimary ? 0 : 1;

        InputAction.PerformInteractiveRebinding()
            .WithTargetBinding(bindingIndex)
            .OnComplete(onComplete)
            .OnCancel(onCancel)
            .Start();
    }
}
