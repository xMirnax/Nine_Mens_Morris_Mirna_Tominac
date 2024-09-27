using UnityEngine;

public class Slot : MonoBehaviour
{

    private void Start()
    {
        BoardManager.Instance.RegisterSlot(this, GridCoordsUtility.GetBoardCoords(SettingsManager.Instance.GetGridConfig(), transform.position));
    }
}