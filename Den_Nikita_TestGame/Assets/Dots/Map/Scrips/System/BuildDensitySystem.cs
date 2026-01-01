using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[BurstCompile]
public partial struct BuildDensitySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (settings, entity) in
                 SystemAPI.Query<RefRO<ChunkSettings>>()
                          .WithAll<NeedDensityBuild>()
                          .WithEntityAccess())
        {
            int w = settings.ValueRO.Width;
            int h = settings.ValueRO.Height;

            int w1 = w + 1;
            int h1 = h + 1;

            var density = state.EntityManager.GetBuffer<Density>(entity);
            density.Clear();
            density.ResizeUninitialized(w1 * h1 * w1);

            // как у тебя: Perlin по x/z, высота до h
            for (int x = 0; x < w1; x++)
                for (int z = 0; z < w1; z++)
                    for (int y = 0; y < h1; y++)
                    {
                        float thisHeight = h * noise.snoise(new float2(
                            (x / 16f) * 1.5f + 0.001f,
                            (z / 16f) * 1.5f + 0.001f
                        ));

                        // В Mono у тебя Mathf.PerlinNoise (0..1).
                        // noise.snoise даёт -1..1 → приводим к 0..1:
                        thisHeight = (thisHeight * 0.5f + 0.5f) * h;

                        float point;
                        if (y <= thisHeight - 0.5f) point = 0f;
                        else if (y > thisHeight + 0.5f) point = 1f;
                        else if (y > thisHeight) point = y - thisHeight;
                        else point = thisHeight - y;

                        int idx = DensityIndex.Idx(x, y, z, w1, h1);
                        density[idx] = new Density { Value = point };
                    }

           
            ecb.RemoveComponent<NeedDensityBuild>(entity);
            ecb.AddComponent<NeedMeshBuild>(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
