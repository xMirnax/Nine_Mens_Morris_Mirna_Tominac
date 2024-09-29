using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
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
    private int[] remainingPieces;

    public UnityEvent<Player> OnPlayerTurnChanged = new UnityEvent<Player>();
    public UnityEvent<GameState> onGameStateChange;
    public UnityEvent onMillFormed;

    private List<Checker> checkersList = new List<Checker>();

    private static readonly int[][] millPatterns = new int[][] {
    new int[] { 0, 1, 2 },
    new int[] { 2, 3, 4 },
    new int[] { 4, 5, 6 },
    new int[] { 6, 7, 0 }
};


    private void OnEnable()
    {
        OnPlayerTurnChanged.AddListener(TurnBegin);
        onMillFormed.AddListener(MillFormed);
    }
    private void OnDisable()
    {
        OnPlayerTurnChanged.RemoveListener(TurnBegin);
        onMillFormed.RemoveListener(MillFormed);
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        currentPlayer = Player.Player1;
        currentState = GameState.PlacementPhase;
        int piecesPerPlayer = SettingsManager.Instance.GetGridConfig().numberOfCheckers;
        remainingPieces = new int[] { piecesPerPlayer, piecesPerPlayer };
        RegisterChecker();
        onGameStateChange?.Invoke(currentState);
        OnPlayerTurnChanged?.Invoke(currentPlayer);
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

    public void PlaceChecker(Checker checker, Vector2Int coord)
    {
        if (currentState != GameState.PlacementPhase)
        {
            Debug.LogWarning("Attempted to place checker outside of placement phase.");
            return;
        }

        if (IsCoordValid(coord))
        {
            int playerIndex = currentPlayer == Player.Player1 ? 0 : 1;
            remainingPieces[playerIndex]--;
            checkers[coord.x, coord.y] = checker;
            if (isMillFormed(checker))
            {
                MillFormed();
            }
            else
            {
                EndTurn();
            }
            if (remainingPieces[0] == 0 && remainingPieces[1] == 0)
            {
                currentState = GameState.MovementPhase;
                onGameStateChange?.Invoke(currentState);
            }
        }
    }

    public void MillFormed()
    {
        currentState = GameState.RemovalPhase;
        onGameStateChange?.Invoke(currentState);
        EnableOpponentCheckerRemoval();
    }

    private void EnableOpponentCheckerRemoval()
    {
        Player opponent = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;
        bool allCheckersInMill = true;

        foreach (Checker checker in checkersList)
        {
            if (checker.player == opponent)
            {
                Vector2Int coord = GetCheckerCoord(checker);

                if (!isMillFormed(checker))
                {
                    checker.SetDraggingEnabled(true);
                    allCheckersInMill = false;
                }
                else
                {
                    checker.SetDraggingEnabled(false);
                }
            }
            else
            {
                checker.SetDraggingEnabled(false);
            }
        }

        if (allCheckersInMill)
        {
            foreach (Checker checker in checkersList)
            {
                if (checker.player == opponent)
                {
                    checker.SetDraggingEnabled(true);
                }
            }
        }
    }

    public bool isMillFormed(Checker checker)
    {
        Vector2Int coord = GetCheckerCoord(checker);
        int ring = coord.x, pos = coord.y;

        if (CheckForMill(ring, pos, checker.player))
        {
            return true;
        }
        return false;
    }

    private bool CheckForMill(int ring, int pos, Player player)
    {

        foreach (var pattern in millPatterns)
        {
            if (pattern.Contains(pos) &&
                checkers[ring, pattern[0]]?.player == player &&
                checkers[ring, pattern[1]]?.player == player &&
                checkers[ring, pattern[2]]?.player == player)
            {
                return true;
            }
        }
        return false;
    }

    public void EndTurn()
    {
        SwitchPlayerTurn();
        OnPlayerTurnChanged.Invoke(currentPlayer);

        foreach (Checker checker in checkersList)
        {
            bool found = false;

            for (int i = 0; i < checkers.GetLength(0); i++)
            {
                for (int j = 0; j < checkers.GetLength(1); j++)
                {
                    if (checkers[i, j] == checker)
                    {
                        found = true;
                        checker.SetDraggingEnabled(false);
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }
        }
    }

    public Checker GetChecker(Vector2Int coord)
    {
        if (IsCoordValid(coord))
        {
            return checkers[coord.x, coord.y];
        }
        return null;
    }

    public Vector2Int GetCheckerCoord(Checker checker)
    {
        if(checker != null)
        {
            for (int row = 0; row < checkers.GetLength(0); row++)
            {
                for (int col = 0; col < checkers.GetLength(1); col++)
                {
                    if (checkers[row, col] == checker)
                    {
                        return new Vector2Int(row, col);
                    }
                }
            }
        }
        return GridCoordsUtility.INVALID_CORD;
    }

    public void RemoveChecker(Checker checker)
    {
        if (currentState == GameState.RemovalPhase && checker.player != currentPlayer)
        {
            Vector2Int coord = GetCheckerCoord(checker);

            if (coord != GridCoordsUtility.INVALID_CORD)
            {
                checkers[coord.x, coord.y] = null;
                checkersList.Remove(checker);
                Destroy(checker.gameObject);

                currentState = GameState.PlacementPhase;
                onGameStateChange?.Invoke(currentState);
                EndTurn();
            }
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
