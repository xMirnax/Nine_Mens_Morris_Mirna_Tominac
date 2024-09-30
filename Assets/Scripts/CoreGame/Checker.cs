using UnityEngine;

public enum Player
{
    Player1,
    Player2
}

public class Checker : MonoBehaviour
{
    public Player player;

    private Vector2 originalPosition;

    private SpriteRenderer spriteRenderer;

    DraggableItem draggableItem;

    private Collider2D checkerCollider;

    public void InitializePlayer(PlayerData data)
    {
        spriteRenderer.color = data.color;
    }

    private void Awake()
    {
        checkerCollider = GetComponent<Collider2D>();
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        draggableItem = GetComponent<DraggableItem>();
        if (draggableItem != null)
        {
            draggableItem.onDrop.AddListener(OnCheckerDrop);
            draggableItem.onPick.AddListener(OnCheckerPick);
        }
    }

    private void OnDisable()
    {
        if (draggableItem != null)
        {
            draggableItem.onDrop.RemoveListener(OnCheckerDrop);
            draggableItem.onPick.RemoveListener(OnCheckerPick);
        }
    }

    void OnCheckerDrop()
    {
        Vector2Int gridCoords = GridCoordsUtility.GetBoardCoords(SettingsManager.Instance.GetGridConfig(), transform.position);

        if (GridCoordsUtility.IsCoordValid(gridCoords) && BoardManager.Instance.CanMoveChecker(this,gridCoords))
        {
            PlaceInSlot(gridCoords);
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    public void OnCheckerPick()
    {
        if(BoardManager.Instance.currentState == GameState.RemovalPhase && this.player != BoardManager.Instance.currentPlayer)
        {
            BoardManager.Instance.RemoveChecker(this);
        }
    }

    public void SetDraggingEnabled(bool enabled)
    {
        draggableItem.SetEnabled(enabled);
        if (checkerCollider != null)
        {
            checkerCollider.enabled = enabled;
        }
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

    public void SetAvaliable(bool avaliable)
    {
        Animator animator = GetComponent<Animator>();
        animator.SetBool("animate", avaliable);
    }
}
