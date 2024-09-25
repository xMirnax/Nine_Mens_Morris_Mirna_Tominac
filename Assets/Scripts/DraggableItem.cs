using UnityEngine;
using UnityEngine.Events;

public class DraggableItem : MonoBehaviour
{
    private static DraggableItem holdingItem = null;

    private bool isDragging = false;
    private Vector2 offset;

    public UnityEvent onPick;
    public UnityEvent onDrop;

    private SpriteRenderer renderer;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {


        if (holdingItem == null)
        {
            renderer.sortingOrder = 1000;
            offset = (Vector2)transform.position - GetMouseWorldPos();
            isDragging = true;
            onPick.Invoke();
            holdingItem = this;
        }

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
        if (holdingItem == this)
        {
            isDragging = false;
            holdingItem = null;
            onDrop.Invoke();
            renderer.sortingOrder = 2;
        }
    }

    private Vector2 GetMouseWorldPos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }


}