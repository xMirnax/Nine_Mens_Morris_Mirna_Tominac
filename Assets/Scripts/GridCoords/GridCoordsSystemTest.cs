using UnityEngine;

public class GridCoordsSystemTest : MonoBehaviour
{
    private GridConfig gridConfig;
    public Color lineColor = Color.white;

    void Start()
    {
        gridConfig = new GridConfig(10f, 10f, 3);
        DrawGrid();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int boardCoords = GridCoordsUtility.GetBoardCoords(gridConfig, mousePosition);

            Debug.Log($"Clicked at Board Position: ({boardCoords.x}, {boardCoords.y})");
        }
    }

    public void DrawGrid()
    {
        for (int ring = 0; ring < gridConfig.numberOfRings; ring++)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2Int coords = new Vector2Int(ring, i);
                Vector2 worldPos = GridCoordsUtility.GetWorldCoords(coords, gridConfig);

                Vector2Int boardCoords = GridCoordsUtility.GetBoardCoords(gridConfig, worldPos);

                Debug.Log($"Grid: ({ring},{i}) -> World: {worldPos} -> Board: {boardCoords}");

                string positionText = $"Grid: {ring},{i}\nBoard: {boardCoords.x},{boardCoords.y}";
                CreateWorldText(transform, positionText, worldPos, 30, Color.white, TextAnchor.MiddleCenter, TextAlignment.Center, 0);

                Vector2Int nextCoords = new Vector2Int(ring, (i + 1) % 8);
                Vector2 nextWorldPos = GridCoordsUtility.GetWorldCoords(nextCoords, gridConfig);

                DrawLineInGameView(worldPos, nextWorldPos, lineColor);
            }
        }
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
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
