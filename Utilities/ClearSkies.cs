using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ProjectM;
using Unity.Entities;

namespace RetroCamera.Utilities;
internal static class ClearSkies
{
    static EntityManager EntityManager => Core.EntityManager;

    static BatFormFog _primaryMaterial;
    static UnityEngine.Material _secondaryMaterial;
    static float _cloudiness;

    public static Entity _dayNightCycleSingleton;
    static bool _isActive = false;

    const float CLOUDINESS = 0.65f;
    const string MATERIAL = "Hidden/Shader/BatFormFog";

    public static void ToggleFog()
    {
        _isActive = !_isActive;

        if (_isActive) RemoveFog();
        else RestoreFog();
    }
    static void RemoveFog()
    {
        if (_primaryMaterial == null) GetBatFormFogMaterial();

        if (_primaryMaterial == null)
        {
            Core.Log.LogWarning($"[ClearSkies] BatFormFog material not found!");
            return;
        }

        _secondaryMaterial = new(_primaryMaterial.m_Material);

        _dayNightCycleSingleton.HasWith((ref DayNightCycle dayNightCycle) =>
        {
            _cloudiness = dayNightCycle.Cloudiness;
            dayNightCycle.Cloudiness = 0f;
        });

        UnityEngine.Object.Destroy(_primaryMaterial.m_Material);
    }
    static void RestoreFog()
    {
        if (_secondaryMaterial == null)
        {
            Core.Log.LogWarning($"[ClearSkies] BatFormFog material not found!");
            return;
        }

        _primaryMaterial.m_Material = new(_secondaryMaterial);

        _dayNightCycleSingleton.With((ref DayNightCycle dayNightCycle) =>
        {
            dayNightCycle.Cloudiness = _cloudiness;
        });
    }
    public static void Initialize()
    {
        GetBatFormFogMaterial();
        // GetDayNightCycleSingleton();
    }
    static void GetBatFormFogMaterial()
    {
        if (_primaryMaterial != null) return;

        Il2CppArrayBase<BatFormFog> batFormFogObjects = UnityEngine.Object.FindObjectsOfType<BatFormFog>();

        _primaryMaterial = batFormFogObjects.LastOrDefault(batFormFog => batFormFog.m_Material != null && batFormFog.m_Material.name.Contains(MATERIAL));
        bool found = _primaryMaterial != null;

        // Core.Log.LogWarning($"[ClearSkies] BatFormFog material found: {found}");
    }
    static void GetDayNightCycleSingleton()
    {
        _dayNightCycleSingleton = Core.UIDataSystem.TryGetSingletonEntity<DayNightCycle>(out Entity singleton) ? singleton : Entity.Null;
        bool exists = _dayNightCycleSingleton.Exists();

        Core.Log.LogWarning($"[ClearSkies] DayNightCycle singleton found: {exists}");
    }
    public static void ResetClearSkies()
    {
        _primaryMaterial = null;
        _secondaryMaterial = null;
        _cloudiness = CLOUDINESS;
        _dayNightCycleSingleton = Entity.Null;
        _isActive = false;
    }
}

