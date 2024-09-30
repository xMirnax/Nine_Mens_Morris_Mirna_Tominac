    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Unity.Collections.LowLevel.Unsafe;
    using UnityEngine;
    using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
    public UnityEvent<GameState> onGameStateChange = new UnityEvent<GameState>();
    public UnityEvent onMillFormed;
    public UnityEvent<Player> OnPlayerWon = new UnityEvent<Player>();

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
        InitializeState();
        StartGame();
    }

    public void ResetState()
    {
        checkers = null;
        slots = null;
        remainingPieces = null;
        checkersList.Clear();
    }

    public void InitializeState()
    {
        currentPlayer = Player.Player1;
        currentState = GameState.PlacementPhase;
        int piecesPerPlayer = gridConfig.numberOfCheckers;
        remainingPieces = new int[] { piecesPerPlayer, piecesPerPlayer };
    }

    public void StartGame()
    {
        RegisterChecker();
        onGameStateChange?.Invoke(currentState);
        OnPlayerTurnChanged?.Invoke(currentPlayer);
    }


    public void TurnBegin(Player playerOnTurn)
    {
        bool allCheckersInMill = AreAllCheckersInMill();

        foreach (Checker checker in checkersList)
        {
            if(checker.player == playerOnTurn)
            {
                checker.SetDraggingEnabled(true);
            }
            else
            {
                checker.SetDraggingEnabled(false);
            }

            checker.SetAvaliable(false);
        }

    }

    public void RegisterChecker()
    {
        checkersList.Clear();

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

    public void PlaceChecker(Checker checker, Vector2Int to)
    {
        if (CanMoveChecker(checker, to))
        {
            if (currentState == GameState.PlacementPhase)
            {
                PlaceCheckerDuringPlacementPhase(checker, to);
            }
            else if (currentState == GameState.MovementPhase)
            {
                MoveChecker(checker, to);
            }
        }
        else
        {
            Debug.LogError("Trying to do invalid move in PlaceChecker");
        }

    }

    public bool CanMoveChecker(Checker checker, Vector2Int to)
    {
        if (currentState == GameState.PlacementPhase)
        {
            return (IsCoordValid(to) && GetChecker(to) == null && GetCheckerCoord(checker) == GridCoordsUtility.INVALID_CORD);
        }
        else if (currentState == GameState.MovementPhase)
        {
            Vector2Int from = GetCheckerCoord(checker);
            return IsValidMove(from, to);
        }
        else 
        {
            Debug.LogError("Invalid State in CanMoveChecker");
            return false; 
        }
    }

    private void PlaceCheckerDuringPlacementPhase(Checker checker, Vector2Int to)
    {
        int playerIndex = currentPlayer == Player.Player1 ? 0 : 1;
        remainingPieces[playerIndex]--;
        checkers[to.x, to.y] = checker;
        if (isMillFormed(checker))
        {
            MillFormed();
        }
        else
        {
            EndTurn();
        }
    }

    private void MoveChecker(Checker checker, Vector2Int to)
    {
        Vector2Int from = GetCheckerCoord(checker);
        checkers[from.x, from.y] = null;

        checkers[to.x, to.y] = checker;
        checker.transform.position = slots[to.x, to.y].transform.position;

        if (isMillFormed(checker))
        {
            MillFormed();
        }
        else
        {
            EndTurn();
        }
    }

    public void MillFormed()
    {
        currentState = GameState.RemovalPhase;
        onGameStateChange?.Invoke(currentState);
        EnableOpponentCheckerRemoval();
    }

    private bool CanCheckerBeRemoved(Checker checker, bool enableInMill)
    {
        bool isOpponent = checker.player != currentPlayer;
        bool isInMill = isMillFormed(checker);
        bool isOnBoard = GridCoordsUtility.IsCoordValid(GetCheckerCoord(checker));
        return isOpponent && (!isInMill || enableInMill) && isOnBoard;
    }

    private bool AreAllCheckersInMill()
    {
        foreach (Checker checker in checkersList)
        {
            Vector2Int coord = GetCheckerCoord(checker);

            if (!isMillFormed(checker) && GridCoordsUtility.IsCoordValid(coord))
            {
                return false;
            }
        }
        return true;
    }

    private void EnableOpponentCheckerRemoval()
    {
        bool allCheckersInMill = AreAllCheckersInMill();
        foreach (Checker checker in checkersList)
        {
            bool canBeRemoved = CanCheckerBeRemoved(checker, allCheckersInMill);
            checker.SetDraggingEnabled(canBeRemoved);
            checker.SetAvaliable(canBeRemoved);
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

        // If there is an between ring mill
        // Only on odd positions
        if(pos % 2 == 1)
        {
            bool allTheSame = true;
            for(int r = 0; r < gridConfig.numberOfRings; r++)
            {
                if(checkers[r, pos] == null || checkers[r, pos].player != player)
                {
                    allTheSame = false;
                    break;
                }
            }
            if (allTheSame)
                return true;
        }


        return false;
    }

    public void PhaseTransition()
    {
        if (remainingPieces[0] == 0 && remainingPieces[1] == 0)
        {
            currentState = GameState.MovementPhase;
            onGameStateChange?.Invoke(currentState);
        }
        else
        {
            currentState = GameState.PlacementPhase;
            onGameStateChange?.Invoke(currentState);
        }
    }

    public void EndTurn()
    {
        SwitchPlayerTurn();
        CheckGameOverConditions();
        PhaseTransition();
        if (currentState != GameState.RemovalPhase) 
        {
            OnPlayerTurnChanged.Invoke(currentPlayer);
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

                PhaseTransition();
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

    public bool IsValidMove(Vector2Int from, Vector2Int to)
    {
        if (!IsCoordValid(from) || !IsCoordValid(to)) 
            return false;


        if (GetChecker(to) != null)
        {
            return false;
        }

        Checker checker = GetChecker(from);
        if (GetRemainingPieces(checker.player) == 3)
        {
            return true;
        }

        // Check if the move is within the same ring
        if (from.x == to.x)
        {
            int diff = Mathf.Abs(from.y - to.y);
            return diff == 1 || diff == 7;
        }

        // Check if the move is between adjacent rings
        if (Mathf.Abs(from.x - to.x) == 1)
        {
            return from.y == to.y;
        }

        return false;
    }


    private int GetRemainingPieces(Player player)
    {
        int count = 0;
        foreach (Checker checker in checkersList)
        {
            if (checker.player == player)
            {
                count++;
            }
        }
        return count;

    }

    private void CheckGameOverConditions()
    {
        int player1Checkers = GetRemainingPieces(Player.Player1);
        int player2Checkers = GetRemainingPieces(Player.Player2);

        if (player1Checkers < 3)
        {
            EndGame(Player.Player2);
        }
        else if (player2Checkers < 3)
        {
            EndGame(Player.Player1);
        }
    }


    private void EndGame(Player winner)
    {
        OnPlayerWon?.Invoke(winner);
        Debug.Log($"Game Over! {winner} wins!");
    }
}
