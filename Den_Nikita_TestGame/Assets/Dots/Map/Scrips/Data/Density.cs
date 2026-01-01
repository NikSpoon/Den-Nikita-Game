using Unity.Entities;
using Unity.Mathematics;

public struct Density : IBufferElementData
{
    public float Value;
}
public static class DensityIndex
{
    public static int Idx(int x, int y, int z, int wPlus1, int hPlus1)
        => x + wPlus1 * (y + hPlus1 * z);
}
public struct MeshVertex : IBufferElementData
{
    public float3 Value;
}

public struct MeshTri : IBufferElementData
{
    public int Value;
}

public struct NeedDensityBuild : IComponentData { }
public struct NeedMeshBuild : IComponentData { }
public struct MeshDirty : IComponentData { }