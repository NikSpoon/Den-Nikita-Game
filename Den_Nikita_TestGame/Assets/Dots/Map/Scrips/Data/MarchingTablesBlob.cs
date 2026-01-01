using Unity.Entities;
using Unity.Mathematics;

public struct MarchingTablesBlob
{
    public BlobArray<int3> CornerTable;      // 8
    public BlobArray<int2> EdgeCornerIndex;  // 12
    public BlobArray<sbyte> TriTable;        // 256 * 16 (sbyte экономит память)
}

public struct MarchingTablesRef : IComponentData
{
    public BlobAssetReference<MarchingTablesBlob> Value;
}

