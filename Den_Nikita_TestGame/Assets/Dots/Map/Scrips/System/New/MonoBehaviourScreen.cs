using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class MonoBehaviourScreen : MonoBehaviour
{
    [Header("Rendering")]
    public Material DefaultMaterial;
    public Texture2D DefaultTexture;

    // ---- Под атлас (позже) ----
    // public Texture2D AtlasTexture;
    // public Vector2Int AtlasTiles = new Vector2Int(4, 4);
    // Материал-индекс лежит в VoxelMaterial (или позже per-triangle mask),
    // а UV кладёшь в MeshUV так, чтобы попадать в нужный tile:
    // uv = (baseUV / AtlasTiles) + tileOffset;
    // tileOffset = new Vector2(tileX, tileY) / AtlasTiles;
    // ----------------------------

    private EntityManager _em;
    private EntityQuery _q;

    private readonly Dictionary<Vector3Int, GameObject> _go = new();
    private readonly Dictionary<Vector3Int, Mesh> _mesh = new();

    static World GetClientWorldOrDefault()
    {
        foreach (var w in World.All)
            if (w != null && w.IsCreated && w.Flags.HasFlag(WorldFlags.GameClient))
                return w;
        return World.DefaultGameObjectInjectionWorld;
    }

    void Start()
    {
        var w = GetClientWorldOrDefault();
        if (w == null || !w.IsCreated) return;

        _em = w.EntityManager;

        _q = _em.CreateEntityQuery(
            ComponentType.ReadOnly<ChunkCoord>(),
            ComponentType.ReadOnly<Unity.Transforms.LocalTransform>(),
            ComponentType.ReadWrite<MeshDirty>(),
            ComponentType.ReadOnly<MeshVertex>(),
            ComponentType.ReadOnly<MeshIndex>()
        );
    }

    void Update()
    {
        if (_em == default) return;

        using var chunks = _q.ToEntityArray(Allocator.Temp);

        foreach (var e in chunks)
        {
            var dirty = _em.GetComponentData<MeshDirty>(e);
            if (dirty.Value == 0) continue;

            var coord = _em.GetComponentData<ChunkCoord>(e).Value;
            var key = new Vector3Int(coord.x, coord.y, coord.z);

            if (!_go.TryGetValue(key, out var go))
            {
                go = new GameObject($"Chunk_{key.x}_{key.y}_{key.z}");
                go.transform.SetParent(transform, false);

                var mf = go.AddComponent<MeshFilter>();
                var mr = go.AddComponent<MeshRenderer>();

                var mesh = new Mesh
                {
                    name = go.name,
                    indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
                };
                mf.sharedMesh = mesh;

                if (DefaultMaterial != null)
                {
                    var mat = new Material(DefaultMaterial);
                    if (DefaultTexture != null) mat.mainTexture = DefaultTexture;

                    // позже:
                    // mat.mainTexture = AtlasTexture;

                    mr.sharedMaterial = mat;
                }

                _go[key] = go;
                _mesh[key] = mesh;
            }

            // позиция чанка
            var lt = _em.GetComponentData<Unity.Transforms.LocalTransform>(e);
            _go[key].transform.position = lt.Position;

            // читаем меш
            var vBuf = _em.GetBuffer<MeshVertex>(e);
            var iBuf = _em.GetBuffer<MeshIndex>(e);

            var m = _mesh[key];
            if (vBuf.Length == 0 || iBuf.Length == 0)
            {
                m.Clear();
                _em.SetComponentData(e, new MeshDirty { Value = 0 });
                continue;
            }

            // (для максимальной оптимизации позже: ArrayPool + Mesh.SetVertexBufferData)
            var verts = new Vector3[vBuf.Length];
            for (int i = 0; i < vBuf.Length; i++)
            {
                var p = vBuf[i].Value;
                verts[i] = new Vector3(p.x, p.y, p.z);
            }

            var tris = new int[iBuf.Length];
            for (int i = 0; i < iBuf.Length; i++) tris[i] = iBuf[i].Value;

            m.Clear();
            m.vertices = verts;
            m.triangles = tris;
            m.RecalculateNormals();
            m.RecalculateBounds();

            _em.SetComponentData(e, new MeshDirty { Value = 0 });
        }
    }
}
