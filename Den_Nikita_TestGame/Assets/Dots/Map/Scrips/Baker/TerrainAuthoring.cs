using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
public class TerrainAuthoring : MonoBehaviour
{
    [SerializeField] private int _width = 32;
    [SerializeField] private int _height = 100;
    [SerializeField] private int _depth = 32;
    public class TerrainBakers : Baker<TerrainAuthoring>
    {

        public override void Bake(TerrainAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity , new TerrainSettingsComponent
            {
                Width = authoring._width,
                Height = authoring._height,
                Depth = authoring._depth
            });

            AddComponent(entity, new MarchingCubesSettings
            {
                Width = authoring._width,
                Height = authoring._height,
                Depth = authoring._depth,
                IsoLevel = 0f,
                CellSize = 1f
            });

        }
    }
}