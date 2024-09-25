using Unity.Burst.CompilerServices;
using UnityEngine;

public class GridCoordsUtility
{
    public static Vector2Int INVALID_CORD = new Vector2Int(-1,-1);


    private static readonly Vector2[] directions = new Vector2[]
    {
        new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(1, 0), new Vector2(1, -1), new Vector2(0, -1),
        new Vector2(-1, -1), new Vector2(-1, 0)
    };

    public static Vector2Int GetBoardCoords(GridConfig gridConfig, Vector2 worldCoords)
    {
        Vector2Int closestCoords = new Vector2Int(-1, -1); // Default value if no valid coords found
        float minDistance = float.MaxValue;

        for (int ring = 0; ring < gridConfig.numberOfRings; ring++)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2Int coords = new Vector2Int(ring, i);
                Vector2 worldPos = GridCoordsUtility.GetWorldCoords(coords, gridConfig);

                float distance = Vector2.Distance(worldCoords, worldPos);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCoords = coords;
                }
            }
        }

        if(minDistance > 20)
        {
            return INVALID_CORD;
        }
        return closestCoords;
    }


    public static Vector2 GetWorldCoords(Vector2Int boardCoords, GridConfig gridConfig)
    {
        int ring = boardCoords.x;
        int position = boardCoords.y;

        float ringSize = (ring + 1) * (2.5f + gridConfig.ringGap); 
        Vector2 direction = directions[position];
        return direction * ringSize;
    }

    public static bool IsCoordValid(Vector2Int boardCoords)
    {
        return boardCoords != INVALID_CORD;
    }
}
