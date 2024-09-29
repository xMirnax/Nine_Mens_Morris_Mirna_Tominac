using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[System.Serializable]
public struct GridConfig
{
    public float gridSize;
    public float ringGap;
    public int numberOfRings;
    public int numberOfCheckers;
}

[System.Serializable]
public struct PlayerData
{
    public string name;
    public Color color;
}

public class SettingsManager : MonoBehaviourSingleton<SettingsManager>
{

    [SerializeField] private GridConfig gridConfig;

    public PlayerData player1;
    public PlayerData player2;


    public PlayerData GetPlayerData(Player player)
    {
        if (player == Player.Player1)
            return player1;
        else 
            return player2;
    }


    public GridConfig GetGridConfig()
    {
        gridConfig.numberOfCheckers = gridConfig.numberOfRings * 3;
        return gridConfig;
    }

}
