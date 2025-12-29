using UnityEngine;

public class MarchingDebuger : Marching
{
    private bool showDebugCubes = false;
    private bool showActiveCubes = false;
    private bool highlightOnHover = true; // Флажок для включения/выключения подсветки при наведении

    private Vector3Int? highlightedCube = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            showDebugCubes = !showDebugCubes;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            showActiveCubes = !showActiveCubes;
        }

        // Проверка наведения курсора на куб, если включена подсветка при наведении
        if (highlightOnHover)
        {
            CheckCubeUnderCursor();
        }

        if (showDebugCubes)
        {
            DrawDebugCubes();
        }

        if (showActiveCubes)
        {
            DrawActiveCubes();
        }

        // Рисуем подсвеченный куб, если он есть
        if (highlightedCube.HasValue)
        {
            DrawHighlightedCube(highlightedCube.Value);
        }
    }

    private void CheckCubeUnderCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 hitPoint = hit.point;
            Vector3Int cubeCoords = new Vector3Int(
                Mathf.FloorToInt(hitPoint.x),
                Mathf.FloorToInt(hitPoint.y),
                Mathf.FloorToInt(hitPoint.z)
            );

            highlightedCube = cubeCoords; // Обновляем подсвеченный куб
        }
        else
        {
            highlightedCube = null; // Если никуда не навели, сбрасываем подсветку
        }
    }

    private void DrawHighlightedCube(Vector3Int center)
    {
        // Ярко-красный цвет для выбранного куба
        DrawCubeAtPosition(center, Color.black);

        // Цвет для соседних кубов
        Color neighborColor = new Color(0.6f, 0.6f, 0.8f);

        // Рисуем соседние кубы
       /* for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dy == 0 && dz == 0) continue;

                    Vector3Int neighbor = center + new Vector3Int(dx, dy, dz);
                    DrawCubeAtPosition(neighbor, neighborColor);
                }
            }
         }
        */
    }


    // Вспомогательный метод для рисования одного куба
    private void DrawCubeAtPosition(Vector3 position, Color color)
    {
        Vector3[] corners = new Vector3[8];
        for (int i = 0; i < 8; i++)
        {
            corners[i] = position + CornerTable[i];
        }

        // Рисуем рёбра куба тем же способом, что и в DrawDebugCubes
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[2], color);
        Debug.DrawLine(corners[2], corners[3], color);
        Debug.DrawLine(corners[3], corners[0], color);
        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[6], color);
        Debug.DrawLine(corners[6], corners[7], color);
        Debug.DrawLine(corners[7], corners[4], color);
        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }
    private void DrawDebugCubes()
    {
        for (int x = 0; x < _wight; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int z = 0; z < _wight; z++)
                {
                    Vector3 cubePos = new Vector3(x, y, z);

                    // Определяем цвет куба в зависимости от его позиции
                    bool isEven = (x + y + z) % 2 == 0;
                    Color cubeColor = isEven ? new Color(0.3f, 0.6f, 0.3f) // Более яркий зелёный
                                             : new Color(0.5f, 0.5f, 0.5f); // Более светлый серый
                    // Рисуем куб с выбранным цветом
                    DrawCubeAtPosition(cubePos, cubeColor);
                }
            }
        }
    }

    private void DrawActiveCubes()
    {
        for (int x = 0; x < _wight; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                for (int z = 0; z < _wight; z++)
                {
                    float[] cube = new float[8];
                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + CornerTable[i];
                        cube[i] = _terraineMap[corner.x, corner.y, corner.z];
                    }

                    int indexConfig = GetCubeCinfiguration(cube);

                    if (indexConfig != 0 && indexConfig != 255)
                    {
                        // Чередуем цвета для активных кубов: тускло-красный и, например, тёмно-синий
                        bool isEven = (x + y + z) % 2 == 0;
                        Color cubeColor = isEven ? new Color(0.7f, 0.2f, 0.2f) // Более яркий красный
                                                 : new Color(0.2f, 0.2f, 0.7f); // Более яркий синий

                        DrawCubeAtPosition(new Vector3(x, y, z), cubeColor);
                    }
                }
            }
        }
    }
}