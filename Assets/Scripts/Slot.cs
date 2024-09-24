using System;
using UnityEngine;

public class Slot : MonoBehaviour
{

    private void OnEnable()
    {
        DraggableItem.onItemDrop += HandleItemDrop;
    }

    public void OnDisable()
    {
        DraggableItem.onItemDrop -= HandleItemDrop;
    }

    private void HandleItemDrop(DraggableItem item, Vector2 boardCoord)
    {
        Vector2Int slotCoords = GridCoordsUtility.GetBoardCoords(SettingsManager.Instance.GetGridConfig(), transform.position);

        if (slotCoords == boardCoord)
        {
            item.transform.position = transform.position;
        }
    }
}
