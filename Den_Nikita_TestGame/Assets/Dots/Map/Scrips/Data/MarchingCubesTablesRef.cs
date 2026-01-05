using Unity.Entities;
using Unity.Mathematics;
public struct MarchingCubesTablesBlob
{
    public BlobArray<int3> CornerTable;        // 8
    public BlobArray<int> EdgeIndexesFlat;    // 12*2
    public BlobArray<int> TriangleTableFlat;  // 256*16
}

public struct MarchingCubesTablesRef : IComponentData
{
    public BlobAssetReference<MarchingCubesTablesBlob> Blob;
}