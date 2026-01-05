using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ChunkWorldSettingsAuthoring : MonoBehaviour
{
    [Header("Chunk Points (voxels)")]
    public int3 Voxels = new int3(16, 16, 16); // это КОЛ-ВО ТОЧЕК, а не cells
    public float CellSize = 1f;
    public float IsoLevel = 0f;

    [Header("Spawn")]
    public int RadiusXZ = 3;        // 7x7, поставь 4 => 9x9
    public int UndergroundY = 4;    // слоёв вниз
    public int SkyY = 0;            // можешь оставить 0 (реально не надо спавнить)

    [Header("Budgets per frame")]
    public int MaxSpawnPerFrame = 16;
    public int MaxTerraformPerFrame = 2;
    public int MaxMeshPerFrame = 1;
    public int MaxPhysicsPerFrame = 1;

    [Header("World Seed")]
    public uint WorldSeed = 12345;

    [Header("Hills")]
    public float HeightAmplitude = 12f;
    public float HeightFrequency = 0.01f;
    public float BaseGroundY = 0f;

    [Header("Caves (underground only)")]
    public float CaveFrequency = 0.05f;
    public float CaveThreshold = 0.55f; // чем больше, тем меньше пустот
    public float CaveStrength = 1.0f;

    [Header("Mesh")]
    public bool InvertWinding = false; // если “вывернуты” треугольники

    public class Baker : Baker<ChunkWorldSettingsAuthoring>
    {
        public override void Bake(ChunkWorldSettingsAuthoring a)
        {
            var e = GetEntity(TransformUsageFlags.None);
            AddComponent(e, new ChunkWorldSettings
            {
                Voxels = a.Voxels,
                CellSize = a.CellSize,
                IsoLevel = a.IsoLevel,

                RadiusXZ = a.RadiusXZ,
                UndergroundY = a.UndergroundY,
                SkyY = a.SkyY,

                MaxSpawnPerFrame = a.MaxSpawnPerFrame,
                MaxTerraformPerFrame = a.MaxTerraformPerFrame,
                MaxMeshPerFrame = a.MaxMeshPerFrame,
                MaxPhysicsPerFrame = a.MaxPhysicsPerFrame,

                WorldSeed = a.WorldSeed,

                HeightAmplitude = a.HeightAmplitude,
                HeightFrequency = a.HeightFrequency,
                BaseGroundY = a.BaseGroundY,

                CaveFrequency = a.CaveFrequency,
                CaveThreshold = a.CaveThreshold,
                CaveStrength = a.CaveStrength,

                InvertWinding = (byte)(a.InvertWinding ? 1 : 0),
            });
        }
    }
}

