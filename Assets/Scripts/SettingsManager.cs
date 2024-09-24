using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GridConfig
{
    public float gridSize;
    public float ringGap;
    public int numberOfRings;
}

public class SettingsManager : MonoBehaviourSingleton<SettingsManager>
{

    [SerializeField] private GridConfig gridConfig;

    public GridConfig GetGridConfig()
    {
        return gridConfig;
    }

}
