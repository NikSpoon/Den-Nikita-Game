using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct BuildMeshSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<MarchingTablesRef>())
            return;

        var tablesRef = SystemAPI.GetSingleton<MarchingTablesRef>().Value;
        ref var t = ref tablesRef.Value;

        foreach (var (settings, entity) in
                 SystemAPI.Query<RefRO<ChunkSettings>>()
                          .WithAll<NeedMeshBuild>()
                          .WithEntityAccess())
        {
            int w = settings.ValueRO.Width;
            int h = settings.ValueRO.Height;
            float surface = settings.ValueRO.Surface;

            int w1 = w + 1;
            int h1 = h + 1;

            var density = state.EntityManager.GetBuffer<Density>(entity);
            var verts = state.EntityManager.GetBuffer<MeshVertex>(entity);
            var tris = state.EntityManager.GetBuffer<MeshTri>(entity);

            verts.Clear();
            tris.Clear();

            // бегаем по "кубам" как у тебя: x<w, y<h, z<w
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    for (int z = 0; z < w; z++)
                    {
                        // 1) считаем конфиг куба (8 углов)
                        int cfg = 0;
                        for (int i = 0; i < 8; i++)
                        {
                            int3 c = t.CornerTable[i];
                            int dx = x + c.x;
                            int dy = y + c.y;
                            int dz = z + c.z;

                            float v = density[DensityIndex.Idx(dx, dy, dz, w1, h1)].Value;
                            if (v > surface) cfg |= 1 << i;
                        }

                        if (cfg == 0 || cfg == 255)
                            continue;

                        // 2) триангуляция: 16 индексов рёбер (как TriangleTable[cfg, ...])
                        int baseTri = cfg * 16;

                        for (int ti = 0; ti < 16; ti++)
                        {
                            sbyte edgeId = t.TriTable[baseTri + ti];
                            if (edgeId == -1) break;

                            // edgeId 0..11 → у него 2 угла (corner indices)
                            int2 edgeCorners = t.EdgeCornerIndex[edgeId];
                            int3 c0 = t.CornerTable[edgeCorners.x];
                            int3 c1 = t.CornerTable[edgeCorners.y];

                            float3 p0 = new float3(x + c0.x, y + c0.y, z + c0.z);
                            float3 p1 = new float3(x + c1.x, y + c1.y, z + c1.z);

                            // как в твоём Mono (midpoint)
                            float3 vpos = (p0 + p1) * 0.5f;

                            int vIndex = verts.Length;
                            verts.Add(new MeshVertex { Value = vpos });
                            tris.Add(new MeshTri { Value = vIndex });
                        }
                    }

            state.EntityManager.RemoveComponent<NeedMeshBuild>(entity);
            if (!state.EntityManager.HasComponent<MeshDirty>(entity))
                state.EntityManager.AddComponent<MeshDirty>(entity);
        }
    }
}
