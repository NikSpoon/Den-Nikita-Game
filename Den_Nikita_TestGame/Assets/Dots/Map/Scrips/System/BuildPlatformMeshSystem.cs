using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
public partial class BuildPlatformMeshSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<MarchingCubesSettings>();
        RequireForUpdate<MarchingCubesTablesRef>();
    }

    protected override void OnUpdate()
    {
        // 0) Настройки + таблицы
        var s = SystemAPI.GetSingleton<MarchingCubesSettings>();

        var tablesEntity = SystemAPI.GetSingletonEntity<MarchingCubesTablesRef>();
        var tablesRef = SystemAPI.GetComponent<MarchingCubesTablesRef>(tablesEntity);
        var tables = tablesRef.Blob;

        // 1) Выходной entity под меш + физику
        if (!SystemAPI.TryGetSingletonEntity<TerrainMeshTag>(out var meshEntity))
        {
            meshEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(meshEntity, new TerrainMeshTag());
            EntityManager.AddComponentData(meshEntity, new MeshDirty { Value = 0 });

            EntityManager.AddBuffer<MeshVertex>(meshEntity);
            EntityManager.AddBuffer<MeshNormal>(meshEntity);
            EntityManager.AddBuffer<MeshIndex>(meshEntity);

            // трансформ нужен для Debug + правильной постановки
            EntityManager.AddComponentData(meshEntity, LocalTransform.Identity);
            EntityManager.AddComponentData(meshEntity, new LocalToWorld { Value = float4x4.identity });

            EntityManager.AddComponent<Simulate>(meshEntity);
            EntityManager.AddSharedComponent(meshEntity, new PhysicsWorldIndex { Value = 0 });
        }
        else
        {
            // Генерим один раз (иначе будешь плодить blob коллайдеры каждый fixed-step)
            if (EntityManager.HasComponent<PhysicsCollider>(meshEntity))
                return;
        }

        var vertices = EntityManager.GetBuffer<MeshVertex>(meshEntity);
        var normals = EntityManager.GetBuffer<MeshNormal>(meshEntity);
        var indices = EntityManager.GetBuffer<MeshIndex>(meshEntity);

        vertices.Clear();
        normals.Clear();
        indices.Clear();

        // ============================================================
        // 2) ПОЛЕ ПЛОТНОСТИ: земля (толстая) + пещеры (много) + входы
        // ============================================================

        // --- Параметры террейна (поверхность) ---
        float baseY = -1f;          // <<< важно (после mapOffset.y)
        float amplitude = 1.0f;    // мелкий рельеф
        float frequency = 0.16f;   // размер неровностей

        // --- Толщина (глубина) земли вниз от поверхности ---
        float groundDepth = 8f;    // тонкий слой земли (плоскость, не огромный куб)

        float SurfaceHeight(float x, float z)
        {
            // [-1..1]
            float n = noise.cnoise(new float2(x, z) * frequency);
            return baseY + amplitude * n;
        }

        // "Толстая земля" вокруг поверхности: положительно внутри слоя
        float GroundDensity(float3 p)
        {
            float surface = SurfaceHeight(p.x, p.z);

            // слой земли: [surface-groundDepth .. surface]
            float center = surface - groundDepth * 0.5f;
            float half = groundDepth * 0.5f;

            return half - math.abs(p.y - center);
        }

        // --- Параметры шумовых пещер (3D noise) ---
        float caveFreq = 0.49f;       // чем меньше — тем крупнее пещеры
        float caveCut = 0.25f;        // больше => пещер меньше
        float caveStrength = 12f;     // насколько сильно "вырезает"

        // Пещеры только в "диапазоне высот" (рандомность по высоте получится из шума + диапазона)
        float caveBandMinY = 10f;
        float caveBandMaxY = 50f;
        float caveBandFade = 18f;      // мягкие границы

        float CaveBandMask(float y)
        {
            // 0 вне диапазона, 1 внутри, с мягким фейдом
            float a = math.saturate((y - caveBandMinY) / caveBandFade);
            float b = math.saturate((caveBandMaxY - y) / caveBandFade);
            return a * b;
        }

        float NoiseCavesField(float3 p)
        {
            float band = CaveBandMask(p.y);
            if (band <= 0f) return 999f; // вне диапазона не режем

            // [-1..1]
            float n = noise.cnoise(p * caveFreq);

            // хотим "воздух" когда n > caveCut => тогда (caveCut - n) отрицательное
            float field = (caveCut - n) * caveStrength;

            // вне бэнда ослабляем (чтобы мягко исчезало)
            return math.lerp(999f, field, band);
        }

        // --- Сферические пещеры (несколько штук) + входы/шахты ---
        // Кол-во пещер можно менять
        const int CaveCount = 8;

        // Детерминированный рандом, чтобы и в Client, и в Server совпадало
        // (seed можно позже вынести в settings/authoring)
        uint seed = (uint)(0x9E3779B9u ^ (uint)(s.Width * 73856093) ^ (uint)(s.Height * 19349663) ^ (uint)(s.Depth * 83492791));
        var rng = new Unity.Mathematics.Random(seed == 0 ? 1u : seed);

        float mapSizeX = (s.Width - 1) * s.CellSize;
        float mapSizeZ = (s.Depth - 1) * s.CellSize;

        // заранее генерим параметры пещер (один раз на билд)
        var caveCenter = new float3[CaveCount];
        var caveRadius = new float[CaveCount];
        var caveHasShaft = new bool[CaveCount];
        var shaftRadius = new float[CaveCount];

        for (int i = 0; i < CaveCount; i++)
        {
            float x = rng.NextFloat(0.1f * mapSizeX, 0.9f * mapSizeX);
            float z = rng.NextFloat(0.1f * mapSizeZ, 0.9f * mapSizeZ);

            // случайная высота пещеры в диапазоне (это то, что ты просил)
            float y = rng.NextFloat(caveBandMinY + 4f, caveBandMaxY - 4f);

            caveCenter[i] = new float3(x, y, z);

            // радиус тоже рандом
            caveRadius[i] = rng.NextFloat(3.5f, 8.5f);

            // у части пещер будет "вход/шахта" к поверхности
            caveHasShaft[i] = rng.NextFloat() < 0.55f;
            shaftRadius[i] = rng.NextFloat(1.2f, 2.8f);
        }

        // Небольшая неровность стен пещеры
        float caveWarpFreq = 0.18f;
        float caveWarpAmp = 1.3f;

        float SphereCavesAndShaftsField(float3 p)
        {
            float best = 999f;

            for (int i = 0; i < CaveCount; i++)
            {
                float3 c = caveCenter[i];
                float r = caveRadius[i];

                // шумовой варп радиуса (делает стены неровными)
                float warp = noise.cnoise(p * caveWarpFreq + (float3)(i * 17.13f)) * caveWarpAmp;

                float dist = math.length(p - c) - (r + warp);
                // dist < 0 => внутри пещеры (воздух)
                best = math.min(best, dist);

                if (caveHasShaft[i])
                {
                    // "Шахта" — цилиндр по Y, от поверхности вниз к пещере:
                    // Внутри цилиндра dist < 0 => воздух (вырезает вход)
                    float2 dxz = new float2(p.x - c.x, p.z - c.z);
                    float cyl = math.length(dxz) - shaftRadius[i];

                    // Ограничим по высоте: только между поверхностью и центром пещеры
                    float surfaceY = SurfaceHeight(c.x, c.z);
                    float yMin = math.min(c.y, surfaceY);
                    float yMax = math.max(c.y, surfaceY);

                    // SDF для "среза" по высоте (мягко):
                    float belowTop = p.y - (yMax + 0.5f);    // >0 выше диапазона
                    float aboveBottom = (yMin - 0.5f) - p.y; // >0 ниже диапазона
                    float yClip = math.max(belowTop, aboveBottom); // >0 вне диапазона

                    // Чтобы цилиндр действовал только внутри [yMin..yMax]
                    float shaftField = math.max(cyl, yClip);

                    best = math.min(best, shaftField);
                }
            }

            return best;
        }
        // Итоговая плотность:
        // - ground: положительно внутри "земли"
        // - caves fields: отрицательно внутри пустот
        // Чтобы "вырезать" пещеры из земли, достаточно взять min()
        float Density(float3 p)
        {
            float ground = GroundDensity(p);

            // шумовые + сферические пещеры/шахты
            float cavesNoise = NoiseCavesField(p);
            float cavesSpheres = SphereCavesAndShaftsField(p);

            float caves = math.min(cavesNoise, cavesSpheres);

            // Вырезаем пещеры из земли
            return math.min(ground, caves);
        }

        // Интерполяция на ребре
        static float3 VertexInterp(float iso, float3 pA, float3 pB, float dA, float dB)
        {
            float denom = dB - dA;
            if (math.abs(denom) < 1e-6f) return pA;
            float t = (iso - dA) / denom;
            return pA + t * (pB - pA);
        }

        // ============================================================
        // 3) Marching Cubes
        // ============================================================

        var pCell = new float3[8];
        var dCell = new float[8];
        var edgeDone = new bool[12];
        var edgeV = new float3[12];
        float3 mapOffset = new float3(
                          (s.Width - 1) * s.CellSize * 0.5f,
                          ((s.Height - 1) * s.CellSize * 0.5f),
                          (s.Depth - 1) * s.CellSize * 0.5f
                          );

        for (int x = 0; x < s.Width - 1; x++)
            for (int y = 0; y < s.Height - 1; y++)
                for (int z = 0; z < s.Depth - 1; z++)
                {
                    for (int c = 0; c < 8; c++)
                    {
                        int3 corner = tables.Value.CornerTable[c];

                        float3 wp = new float3(x + corner.x, y + corner.y, z + corner.z) * s.CellSize - mapOffset;
                        pCell[c] = wp;
                        dCell[c] = Density(wp);
                    }

                    // cubeIndex (>= чтобы не терять границу)
                    int cubeIndex = 0;
                    if (dCell[0] >= s.IsoLevel) cubeIndex |= 1;
                    if (dCell[1] >= s.IsoLevel) cubeIndex |= 2;
                    if (dCell[2] >= s.IsoLevel) cubeIndex |= 4;
                    if (dCell[3] >= s.IsoLevel) cubeIndex |= 8;
                    if (dCell[4] >= s.IsoLevel) cubeIndex |= 16;
                    if (dCell[5] >= s.IsoLevel) cubeIndex |= 32;
                    if (dCell[6] >= s.IsoLevel) cubeIndex |= 64;
                    if (dCell[7] >= s.IsoLevel) cubeIndex |= 128;

                    int triBase = cubeIndex * 16;
                    if (tables.Value.TriangleTableFlat[triBase] == -1)
                        continue;

                    for (int e = 0; e < 12; e++)
                        edgeDone[e] = false;

                    float3 GetEdgeVertex(int edgeId)
                    {
                        if (edgeDone[edgeId]) return edgeV[edgeId];

                        int a = tables.Value.EdgeIndexesFlat[edgeId * 2 + 0];
                        int b = tables.Value.EdgeIndexesFlat[edgeId * 2 + 1];

                        float3 v = VertexInterp(s.IsoLevel, pCell[a], pCell[b], dCell[a], dCell[b]);
                        edgeV[edgeId] = v;
                        edgeDone[edgeId] = true;
                        return v;
                    }

                    for (int i = 0; i < 16; i += 3)
                    {
                        int e0 = tables.Value.TriangleTableFlat[triBase + i + 0];
                        if (e0 == -1) break;

                        int e1 = tables.Value.TriangleTableFlat[triBase + i + 1];
                        int e2 = tables.Value.TriangleTableFlat[triBase + i + 2];

                        float3 v0 = GetEdgeVertex(e0);
                        float3 v1 = GetEdgeVertex(e1);
                        float3 v2 = GetEdgeVertex(e2);

                        // ✅ ВАЖНО: winding под Unity (v0, v2, v1)
                        float3 n = math.normalizesafe(math.cross(v2 - v0, v1 - v0), new float3(0, 1, 0));

                        int baseIndex = vertices.Length;

                        vertices.Add(new MeshVertex { Value = v0 });
                        vertices.Add(new MeshVertex { Value = v2 });
                        vertices.Add(new MeshVertex { Value = v1 });

                        normals.Add(new MeshNormal { Value = n });
                        normals.Add(new MeshNormal { Value = n });
                        normals.Add(new MeshNormal { Value = n });

                        indices.Add(new MeshIndex { Value = baseIndex + 0 });
                        indices.Add(new MeshIndex { Value = baseIndex + 1 });
                        indices.Add(new MeshIndex { Value = baseIndex + 2 });
                    }
                }

        Debug.Log($"MC Mesh built: v={vertices.Length} i={indices.Length}");

        // 4) Пометить меш грязным для Mono-рендера
        EntityManager.SetComponentData(meshEntity, new MeshDirty { Value = 1 });

        // 5) Создать Unity.Physics MeshCollider
        if (vertices.Length >= 3 && indices.Length >= 3)
        {
            var v = new NativeArray<float3>(vertices.Length, Allocator.Temp);
            for (int i = 0; i < vertices.Length; i++)
                v[i] = vertices[i].Value;

            var t = new NativeArray<int3>(indices.Length / 3, Allocator.Temp);
            for (int i = 0; i < t.Length; i++)
            {
                int i0 = indices[i * 3 + 0].Value;
                int i1 = indices[i * 3 + 1].Value;
                int i2 = indices[i * 3 + 2].Value;
                t[i] = new int3(i0, i1, i2);
            }

            var filter = new CollisionFilter
            {
                BelongsTo = 1u,
                CollidesWith = 1u,
                GroupIndex = 0
            };

            var collider = Unity.Physics.MeshCollider.Create(v, t, filter);
            EntityManager.AddComponentData(meshEntity, new PhysicsCollider { Value = collider });

            v.Dispose();
            t.Dispose();
        }
        else
        {
            Debug.LogWarning("MC: mesh is empty => collider not created");
        }
    }
}
