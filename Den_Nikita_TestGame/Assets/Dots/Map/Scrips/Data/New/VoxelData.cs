using Unity.Entities;
using Unity.Mathematics;

// ---- Воксельные буферы (Marching Cubes требует (voxels+1)^3 точек) ----
public struct VoxelDensity : IBufferElementData { public float Value; }
public struct VoxelMaterial : IBufferElementData { public byte Value; } // под атлас/материал (позже)

// ---- Мешевые буферы ----
public struct MeshDirty : IComponentData { public byte Value; }
public struct MeshVertex : IBufferElementData { public float3 Value; }
public struct MeshNormal : IBufferElementData { public float3 Value; }
public struct MeshIndex : IBufferElementData { public int Value; }
public struct MeshUV : IBufferElementData { public float2 Value; }