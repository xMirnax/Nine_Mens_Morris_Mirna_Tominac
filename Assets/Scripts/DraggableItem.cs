using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableItem : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    public static event Action<DraggableItem, Vector2> onItemDrop;

    private void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int boardCoords = GridCoordsUtility.GetBoardCoords(SettingsManager.Instance.GetGridConfig(), GetMouseWorldPos());

            onItemDrop?.Invoke(this, boardCoords);

            isDragging = false;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
