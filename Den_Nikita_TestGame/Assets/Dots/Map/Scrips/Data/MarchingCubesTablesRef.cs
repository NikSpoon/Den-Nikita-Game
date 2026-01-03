using Unity.Entities;
using Unity.Mathematics;

public struct MarchingCubesTablesBlob
{
    // 8 углов (как int3 вместо Vector3Int)
    public BlobArray<int3> CornerTable;     // length = 8

    // 12 рёбер, каждое ребро = 2 индекса углов => length = 12*2 = 24
    public BlobArray<int> EdgeIndexesFlat;  // [edge*2 + 0/1]

    // 256 кейсов, в каждом до 16 индексов => length = 256*16 = 4096
    public BlobArray<int> TriangleTableFlat; // [case*16 + i]
}

public struct MarchingCubesTablesRef : IComponentData
{
    public BlobAssetReference<MarchingCubesTablesBlob> Blob;
}
