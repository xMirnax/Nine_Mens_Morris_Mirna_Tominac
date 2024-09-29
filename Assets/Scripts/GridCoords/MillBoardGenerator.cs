using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class MillBoardGenerator : MonoBehaviour
{
    public Color lineColor = Color.white;
    public Slot slotPrefab;
    public Checker checkerPrefab;
    [SerializeField] private GameObject linesParent;
    [SerializeField] private Transform player1Checkers;
    [SerializeField] private Transform player2Checkers;

    public float startingCheckerOffset = 0.0f;

    GridConfig gridConfig;

    void Start()
    {
        gridConfig = SettingsManager.Instance.GetGridConfig();
        BoardManager.Instance.InitializeBoard(gridConfig);
        DrawGrid();
        InstantiateCheckers(player1Checkers, Player.Player1);
        InstantiateCheckers(player2Checkers, Player.Player2);
    
    }

    public void InstantiateCheckers(Transform startPosition, Player player)
    {
        int columns = 3;
        int checkersPerColumn = gridConfig.numberOfCheckers / columns;
        for (int checkers = 0; checkers < gridConfig.numberOfCheckers; checkers++)
        {
            int column = checkers % columns;
            int row = checkers / columns;
            float xOffset = (column - 1) * 5 + startingCheckerOffset;
            Vector2 offset = new Vector2(xOffset, row * 5);
            Checker instance = Instantiate(checkerPrefab, (Vector2)startPosition.position + offset, Quaternion.identity, transform);
            instance.player = player;
            instance.InitializePlayer(SettingsManager.Instance.GetPlayerData(player));
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
                InstantiatePrefab(worldPos, $"{boardCoords.x},{boardCoords.y}");
                Vector2Int nextCoords = new Vector2Int(ring, (i + 1) % 8);
                Vector2 nextWorldPos = GridCoordsUtility.GetWorldCoords(nextCoords, gridConfig);
                DrawLineInGameView(worldPos, nextWorldPos, lineColor);
            }
        }
    }

    private void InstantiatePrefab(Vector3 position, string labelText)
    {
        if (slotPrefab != null)
        {
            Slot instance = Instantiate(slotPrefab, position, Quaternion.identity, transform);
            instance.name = $"GridObject_{labelText}";
            BoardManager.Instance.RegisterSlot(instance, GridCoordsUtility.GetBoardCoords(SettingsManager.Instance.GetGridConfig(), position));
        }
    }

    private void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
    {
        GameObject lineObject = new GameObject("Line");
        lineObject.transform.parent = linesParent.transform;
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 1;
        lineRenderer.endWidth = 1;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.sortingOrder = 1;
    }
}