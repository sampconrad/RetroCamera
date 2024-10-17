using ProjectM;
using static ModernCamera.Utilities.StateUtilities;

namespace ModernCamera.Behaviours;
internal class FirstPersonCameraBehaviour : CameraBehaviour
{
    public FirstPersonCameraBehaviour()
    {
        BehaviourType = BehaviourType.FirstPerson;
        DefaultMaxPitch = 1.57f;
        DefaultMinPitch = -1.57f;
    }
    public override void Activate(ref TopdownCameraState state)
    {
        base.Activate(ref state);

        IsMouseLocked = true;
        IsFirstPerson = true;

        CurrentBehaviourType = BehaviourType;
        state.PitchPercent = 0.51f;
        TargetZoom = 0;
    }
    public override void Deactivate()
    {
        base.Deactivate();

        if (!IsActionMode) IsMouseLocked = false;
        IsFirstPerson = false;
    }
    public override bool ShouldActivate(ref TopdownCameraState state)
    {
        return Settings.FirstPersonEnabled && CurrentBehaviourType != BehaviourType && TargetZoom < Settings.MinZoom;
    }
    public override void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        base.UpdateCameraInputs(ref state, ref data);

        var forwardOffset = Settings.FirstPersonForwardOffset;
        var headHeight = Settings.HeadHeightOffset;

        if (Settings.FirstPersonShapeshiftOffsets.ContainsKey(shapeshiftName))
        {
            forwardOffset = Settings.FirstPersonShapeshiftOffsets[shapeshiftName].y;
            headHeight = Settings.FirstPersonShapeshiftOffsets[shapeshiftName].x;
        }

        state.LastTarget.NormalizedLookAtOffset.z = forwardOffset;
        state.LastTarget.NormalizedLookAtOffset.y = IsMounted ? headHeight + Settings.MountedOffset : headHeight;
    }
}
