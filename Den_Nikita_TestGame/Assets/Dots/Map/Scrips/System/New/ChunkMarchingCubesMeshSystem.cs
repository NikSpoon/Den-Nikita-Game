using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[UpdateInGroup(typeof(ChunkPipelineGroup))]
[UpdateAfter(typeof(ChunkTerraformSystem))]
public partial class ChunkMarchingCubesMeshSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<ChunkWorldSettings>();
        RequireForUpdate<MarchingCubesTablesRef>();
    }

    protected override void OnUpdate()
    {
        var s = SystemAPI.GetSingleton<ChunkWorldSettings>();
        var tables = SystemAPI.GetSingleton<MarchingCubesTablesRef>();
        int builtThisFrame = 0;

        foreach (var (coordRO, stateRW, entity) in SystemAPI
                     .Query<RefRO<ChunkCoord>, RefRW<ChunkState>>()
                     .WithNone<SkyChunkTag>()
                     .WithEntityAccess())
        {
            if (builtThisFrame >= s.MaxMeshPerFrame)
                break;

            if (stateRW.ValueRO.Stage != ChunkStage.Terraformed)
                continue;

            // ✅ Мешим и поверхность и подземку (y<=0)
            if (coordRO.ValueRO.Value.y > 0)
                continue;

            var density = EntityManager.GetBuffer<VoxelDensity>(entity);

            var v = EntityManager.GetBuffer<MeshVertex>(entity);
            var n = EntityManager.GetBuffer<MeshNormal>(entity);
            var uv = EntityManager.GetBuffer<MeshUV>(entity);
            var t = EntityManager.GetBuffer<MeshIndex>(entity);

            v.Clear(); n.Clear(); uv.Clear(); t.Clear();

            BuildMarchingCubes(
                s, tables.Blob,
                density,
                v, n, uv, t
            );

            EntityManager.SetComponentData(entity, new MeshDirty { Value = 1 });
            stateRW.ValueRW.Stage = ChunkStage.Meshed;
            builtThisFrame++;
        }
    }

    private static void BuildMarchingCubes(
        in ChunkWorldSettings s,
        BlobAssetReference<MarchingCubesTablesBlob> tableBlob,
        DynamicBuffer<VoxelDensity> density,
        DynamicBuffer<MeshVertex> outV,
        DynamicBuffer<MeshNormal> outN,
        DynamicBuffer<MeshUV> outUV,
        DynamicBuffer<MeshIndex> outI)
    {
        int3 vox = s.Voxels;           // points
        int3 cells = vox - 1;          // cells

        ref var blob = ref tableBlob.Value;
        ref var corners = ref blob.CornerTable;
        ref var edges = ref blob.EdgeIndexesFlat;
        ref var triTable = ref blob.TriangleTableFlat;

        float iso = s.IsoLevel;
        float cs = s.CellSize;
        bool invert = s.InvertWinding != 0;

        // 12 edge verts per cell
        FixedList512Bytes<float3> edgeVert = default;

        int Index(int x, int y, int z) => (z * vox.y + y) * vox.x + x;

        for (int cz = 0; cz < cells.z; cz++)
            for (int cy = 0; cy < cells.y; cy++)
                for (int cx = 0; cx < cells.x; cx++)
                {
                    // 8 corner densities + positions
                  
                    float[] cornerD = new float[8]; // (можно оптимизировать дальше, но ок для старта)

                    float3[] cornerP = new float3[8];

                    for (int i = 0; i < 8; i++)
                    {
                        int3 c = corners[i];
                        int px = cx + c.x;
                        int py = cy + c.y;
                        int pz = cz + c.z;

                        cornerP[i] = new float3(px * cs, py * cs, pz * cs);
                        cornerD[i] = density[Index(px, py, pz)].Value;
                    }

                    // case index
                    int caseIndex = 0;
                    for (int i = 0; i < 8; i++)
                        if (cornerD[i] > iso) caseIndex |= (1 << i);

                    // если кейс пустой или полный
                    int triBase = caseIndex * 16;
                    if (triTable[triBase] == -1)
                        continue;

                    // compute 12 edge intersections
                    edgeVert.Length = 12;

                    for (int e = 0; e < 12; e++)
                    {
                        int a = edges[e * 2 + 0];
                        int b = edges[e * 2 + 1];

                        float da = cornerD[a] - iso;
                        float db = cornerD[b] - iso;

                        float3 pa = cornerP[a];
                        float3 pb = cornerP[b];

                        float t = 0.5f;
                        float denom = (da - db);
                        if (math.abs(denom) > 1e-6f)
                            t = da / (da - db);

                        t = math.clamp(t, 0f, 1f);
                        edgeVert[e] = pa + (pb - pa) * t;
                    }

                    // triangles
                    for (int i = 0; i < 16; i += 3)
                    {
                        int e0 = triTable[triBase + i];
                        if (e0 == -1) break;

                        int e1 = triTable[triBase + i + 1];
                        int e2 = triTable[triBase + i + 2];

                        float3 p0 = edgeVert[e0];
                        float3 p1 = edgeVert[e1];
                        float3 p2 = edgeVert[e2];

                        int baseIndex = outV.Length;

                        // winding
                        if (!invert)
                        {
                            outV.Add(new MeshVertex { Value = p0 });
                            outV.Add(new MeshVertex { Value = p1 });
                            outV.Add(new MeshVertex { Value = p2 });
                        }
                        else
                        {
                            outV.Add(new MeshVertex { Value = p0 });
                            outV.Add(new MeshVertex { Value = p2 });
                            outV.Add(new MeshVertex { Value = p1 });
                        }

                        // нормаль face
                        float3 a = outV[baseIndex + 1].Value - outV[baseIndex + 0].Value;
                        float3 b = outV[baseIndex + 2].Value - outV[baseIndex + 0].Value;
                        float3 normal = math.normalize(math.cross(a, b));

                        outN.Add(new MeshNormal { Value = normal });
                        outN.Add(new MeshNormal { Value = normal });
                        outN.Add(new MeshNormal { Value = normal });

                        // UV пока простые (под атлас потом заменишь)
                        outUV.Add(new MeshUV { Value = new float2(0, 0) });
                        outUV.Add(new MeshUV { Value = new float2(1, 0) });
                        outUV.Add(new MeshUV { Value = new float2(0, 1) });

                        outI.Add(new MeshIndex { Value = baseIndex + 0 });
                        outI.Add(new MeshIndex { Value = baseIndex + 1 });
                        outI.Add(new MeshIndex { Value = baseIndex + 2 });
                    }
                }
    }
}
