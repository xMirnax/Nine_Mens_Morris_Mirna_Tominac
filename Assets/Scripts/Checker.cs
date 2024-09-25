using UnityEngine;

public class Checker : MonoBehaviour
{

    private Vector2 originalPosition;

    private void Start()
    {
        BoardManager.Instance.RegisterChecker(this);
    }


    private void Awake()
    {
        originalPosition = transform.position;
    }

    private void OnEnable()
    {
        DraggableItem draggableItem = GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            draggableItem.onDrop.AddListener(OnCheckerDrop);
            draggableItem.onPick.AddListener(OnCheckerPick);
        }
    }

    private void OnDisable()
    {
        DraggableItem draggableItem = GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            draggableItem.onDrop.RemoveListener(OnCheckerDrop);
            draggableItem.onPick.RemoveListener(OnCheckerPick);
        }
    }

    void OnCheckerDrop()
    {
        Vector2Int gridCoords = GridCoordsUtility.GetBoardCoords(SettingsManager.Instance.GetGridConfig(), transform.position);

        if (GridCoordsUtility.IsCoordValid(gridCoords))
        {
            PlaceInSlot(gridCoords);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private void OnCheckerPick()
    {
    }


    private void PlaceInSlot(Vector2Int gridCoords)
    {
        BoardManager boardManager = BoardManager.Instance;
        if (boardManager.GetChecker(gridCoords) == null)
        {
            Vector2 worldPosition = GridCoordsUtility.GetWorldCoords(gridCoords, SettingsManager.Instance.GetGridConfig());
            transform.position = worldPosition;

            boardManager.PlaceChecker(this, gridCoords);
            SetOriginalPosition(transform.position);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private void ReturnToOriginalPosition()
    {
        transform.position = originalPosition;
    }

    public void SetOriginalPosition(Vector2 position)
    {
        originalPosition = position;
    }
}
