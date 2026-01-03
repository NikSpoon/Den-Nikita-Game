using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using System.Collections;
using Unity.Collections;

public class BoxMeshSpawner : MonoBehaviour
{
    public GameObject boxPrefab; // Префаб куба

    private EntityManager _entityManager;
    private EntityQuery _query;

    void Start()
    {
        // Получаем EntityManager из текущего мира
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Запускаем корутину с небольшой задержкой
        StartCoroutine(SpawnBoxesWithDelay());
    }

    private IEnumerator SpawnBoxesWithDelay()
    {
        // Ждём один кадр, чтобы дать ECS время на создание сущностей
        yield return null;

        // Теперь создаём запрос
        _query = _entityManager.CreateEntityQuery(typeof(BoxMapTeg), typeof(LocalToWorld));

        // Получаем все сущности с этим тегом
        var entities = _query.ToEntityArray(Allocator.TempJob);

        // Создаём кубы для каждой сущности
        foreach (var entity in entities)
        {
            var ltw = _entityManager.GetComponentData<LocalToWorld>(entity);
            Vector3 position = ltw.Position;

            // Инстанцируем префаб на позиции
            Instantiate(boxPrefab, position, Quaternion.identity);
        }

        // Освобождаем массив
        entities.Dispose();
    }
}
