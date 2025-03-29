using ProjectM;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera.Behaviours;
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

        _isMouseLocked = true;
        _isFirstPerson = true;
        _currentBehaviourType = BehaviourType;

        state.PitchPercent = 0.51f;
        _targetZoom = 0;
    }
    public override void Deactivate()
    {
        base.Deactivate();

        if (!_isActionMode) _isMouseLocked = false;
        _isFirstPerson = false;
    }
    public override bool ShouldActivate(ref TopdownCameraState state)
    {
        return Settings.FirstPersonEnabled && _currentBehaviourType != BehaviourType && _targetZoom < Settings.MinZoom;
    }
    public override void UpdateCameraInputs(ref TopdownCameraState state, ref TopdownCamera data)
    {
        base.UpdateCameraInputs(ref state, ref data);

        float forwardOffset = Settings.FIRST_PERSON_FORWARD_OFFSET;
        float headHeight = Settings.HEAD_HEIGHT_OFFSET;

        if (Settings.FirstPersonShapeshiftOffsets.ContainsKey(_shapeshiftName))
        {
            forwardOffset = Settings.FirstPersonShapeshiftOffsets[_shapeshiftName].y;
            headHeight = Settings.FirstPersonShapeshiftOffsets[_shapeshiftName].x;
        }

        state.LastTarget.NormalizedLookAtOffset.z = forwardOffset;
        state.LastTarget.NormalizedLookAtOffset.y = _isMounted ? headHeight + Settings.MOUNTED_OFFSET : headHeight;
    }
}
