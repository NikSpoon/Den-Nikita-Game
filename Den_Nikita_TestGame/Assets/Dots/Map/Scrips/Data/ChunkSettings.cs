using Unity.Entities;

public struct ChunkSettings : IComponentData
{
    public int Width;    // 32
    public int Height;   // 10
    public float Surface; // 0.5
    public int Seed;
}
