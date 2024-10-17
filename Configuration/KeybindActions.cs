using ProjectM;
using System.Text;
using UnityEngine;
using static ModernCamera.Configuration.KeybindsManager;

namespace ModernCamera.Configuration;

public delegate void KeyEvent();
internal class KeybindActions
{
    public static event KeyEvent KeyPressed = delegate { };
    public static event KeyEvent KeyDown = delegate { };
    public static event KeyEvent KeyUp = delegate { };
    public class KeybindMapping(KeybindingDescription description)
    {
        public KeybindingDescription Description = description;
        public KeyCode Primary = description.DefaultKeybind;
        public KeyCode Secondary = KeyCode.None;

        public ButtonInputAction InputFlag = ComputeInputFlag(description.Name);
        public int AssetGuid = ComputeAssetGuid(description.Name);
        public bool IsPressed => Input.GetKey(Primary) || Input.GetKey(Secondary);
        public bool IsDown => Input.GetKeyDown(Primary) || Input.GetKeyDown(Secondary);
        public bool IsUp => Input.GetKeyUp(Primary) || Input.GetKeyUp(Secondary);
        public void AddKeyPressedListener(KeyEvent action) => KeyPressed += action;
        public void AddKeyDownListener(KeyEvent action) => KeyDown += action;
        public void AddKeyUpListener(KeyEvent action) => KeyUp += action;
        public void OnKeyPressed() => KeyPressed();
        public void OnKeyDown() => KeyDown();
        public void OnKeyUp() => KeyUp();
    }

    // Hash computation for InputFlag and AssetGuid
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

    // Method for rebinding
    /*
    public void StartRebinding(bool isPrimary, Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> onComplete, Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> onCancel)
    {
        int bindingIndex = isPrimary ? 0 : 1;

        InputAction.PerformInteractiveRebinding()
            .WithTargetBinding(bindingIndex)
            .OnComplete(onComplete)
            .OnCancel(onCancel)
            .Start();
    }
    */
}
