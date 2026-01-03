using Unity.Entities;

public struct TerrainSettingsComponent : IComponentData
{
    public int Width;
    public int Height;
    public int Depth;
}