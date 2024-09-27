using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    PlacementPhase,
    MovementPhase,
    RemovalPhase
}

public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    private Checker[,] checkers;
    private Slot[,] slots;
    private GridConfig gridConfig;
    public Player currentPlayer { get; private set; }
    public GameState currentState { get; private set; }
    public int remainingPieces { get; private set; }

    [SerializeField] private UnityEvent<Player> onTurnBegin = new UnityEvent<Player>();
    [SerializeField] UnityEvent<GameState> onGameStateChange;

    private List<Checker> checkersList = new List<Checker>();

    private void OnEnable()
    {
        onTurnBegin.AddListener(TurnBegin);
    }
    private void OnDisable()
    {
        onTurnBegin.RemoveListener(TurnBegin);
    }

    private void Start()
    {
        currentPlayer = Player.Player1;
        RegisterChecker();

        onTurnBegin?.Invoke(currentPlayer);
    }


    public void TurnBegin(Player playerOnTurn)
    {
        foreach (Checker checker in checkersList)
        {
            checker.DraggingEnabler(playerOnTurn);
        }

    }

    public void RegisterChecker()
    {
        Checker[] checkersArray = FindObjectsOfType<Checker>();
        foreach (Checker checker in checkersArray)
        {
            checkersList.Add(checker);
        }
    }

    public void InitializeBoard(GridConfig config)
    {
        gridConfig = config;
        checkers = new Checker[config.numberOfRings, 8];
        slots = new Slot[config.numberOfRings, 8];
    }

    public void RegisterSlot(Slot slot, Vector2Int gridCoord)
    {
        if (IsCoordValid(gridCoord))
        {
            slots[gridCoord.x, gridCoord.y] = slot;
        }
    }

    public Slot GetSlot(Vector2Int gridCoord)
    {
        if (IsCoordValid(gridCoord))
        {
            return slots[gridCoord.x, gridCoord.y];
        }
        return null;
    }

    public void PlaceChecker(Checker checker, Vector2Int coord)
    {
        if (IsCoordValid(coord))
        {
            checkers[coord.x, coord.y] = checker;
            EndTurn();
        }
    }

    public void EndTurn()
    {
        SwitchPlayerTurn();
        onTurnBegin.Invoke(currentPlayer);
    }

    public Checker GetChecker(Vector2Int coord)
    {
        if (IsCoordValid(coord))
        {
            return checkers[coord.x, coord.y];
        }
        return null;
    }

    public void RemoveChecker(Vector2Int coord)
    {
        if (IsCoordValid(coord))
        {
            checkers[coord.x, coord.y] = null;
        }
    }

    private bool IsCoordValid(Vector2Int coord)
    {
        return coord.x >= 0 && coord.x < gridConfig.numberOfRings && coord.y >= 0 && coord.y < 8;
    }

    private void SwitchPlayerTurn()
    {
        currentPlayer = currentPlayer == Player.Player1 ? Player.Player2 : Player.Player1;
    }

}
