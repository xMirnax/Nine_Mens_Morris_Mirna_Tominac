using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviourSingleton<BoardManager>
{
    private Dictionary<Checker, Vector2Int> checkers = new Dictionary<Checker, Vector2Int>();
    private Dictionary<Vector2Int, Slot > slots = new Dictionary<Vector2Int, Slot>();


    public void RegisterSlot(Slot slot, Vector2Int gridCoord)
    {
        slots[gridCoord] = slot;
    }

    public void RegisterChecker(Checker checker)
    {
        checkers[checker] = GridCoordsUtility.INVALID_CORD;
    }

    public Slot GetSlot(Vector2Int gridCoord)
    {
        if(GridCoordsUtility.IsCoordValid(gridCoord))
        {
            return slots[gridCoord];
        }
        return null;
    }

    public void PlaceChecker(Checker checker, Vector2Int coord)
    {
        checkers[checker] = coord;
    }

    public Checker GetChecker(Vector2Int coord)
    {
        foreach (var entry in checkers)
        {
            if (entry.Value == coord)
            {
                return entry.Key;
            }
        }
        return null;
    }

}
