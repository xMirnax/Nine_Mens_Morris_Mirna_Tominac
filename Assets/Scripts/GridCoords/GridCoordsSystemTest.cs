using UnityEngine;

public class GridCoordsSystemTest : MonoBehaviour
{
    public Color lineColor = Color.white;
    public GameObject prefab;

    void Start()
    {
        DrawGrid();
    }

    public void DrawGrid()
    {
        GridConfig gridConfig = SettingsManager.Instance.GetGridConfig();

        for (int ring = 0; ring < gridConfig.numberOfRings; ring++)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2Int coords = new Vector2Int(ring, i);
                Vector2 worldPos = GridCoordsUtility.GetWorldCoords(coords, gridConfig);
                Vector2Int boardCoords = GridCoordsUtility.GetBoardCoords(gridConfig, worldPos);
                //Debug.Log($"Grid: ({ring},{i}) -> World: {worldPos} -> Board: {boardCoords}");

                InstantiatePrefab(worldPos, $"Grid: {ring},{i}\nBoard: {boardCoords.x},{boardCoords.y}");

                Vector2Int nextCoords = new Vector2Int(ring, (i + 1) % 8);
                Vector2 nextWorldPos = GridCoordsUtility.GetWorldCoords(nextCoords, gridConfig);
                DrawLineInGameView(worldPos, nextWorldPos, lineColor);
            }
        }
    }

    private void InstantiatePrefab(Vector3 position, string labelText)
    {
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, position, Quaternion.identity, transform);
            instance.name = $"GridObject_{labelText}";
        }
    }

    private void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
    {
        GameObject lineObject = new GameObject("Line");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.sortingOrder = 1;
    }
}