
using Unity.Entities;
using Unity.Mathematics;
// --- World settings singleton ---

public struct ChunkWorldSettings : IComponentData
{
    public int3 Voxels;
    public float CellSize;
    public float IsoLevel;

    public int RadiusXZ;
    public int UndergroundY;
    public int SkyY;

    public int MaxSpawnPerFrame;
    public int MaxTerraformPerFrame;
    public int MaxMeshPerFrame;
    public int MaxPhysicsPerFrame;

    public uint WorldSeed;

    public float HeightAmplitude;
    public float HeightFrequency;
    public float BaseGroundY;

    public float CaveFrequency;
    public float CaveThreshold;
    public float CaveStrength;

    public byte InvertWinding;
}
public struct ChunkCoord : IComponentData { public int3 Value; } // (cx, cy, cz)

public enum ChunkStage : byte
{
    Spawned = 0,
    Terraformed = 1,
    Meshed = 2,
    PhysicsBuilt = 3
}
public struct ChunkState : IComponentData { public ChunkStage Stage; }

public struct ChunkSpawnedTag : IComponentData { }
public struct SkyChunkTag : IComponentData { }          // y > 0 (мы не мешим)
public struct UndergroundChunkTag : IComponentData { }  // y < 0
public struct SurfaceChunkTag : IComponentData { }      // y == 0
