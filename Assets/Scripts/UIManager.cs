using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;

    [SerializeField] private TextMeshProUGUI turnIndicatorText;
    [SerializeField] private TextMeshProUGUI phaseIndicatorText;

    private void OnEnable()
    {
        BoardManager.Instance.onGameStateChange.AddListener(StateChange);
        BoardManager.Instance.OnPlayerTurnChanged.AddListener(PlayerTurn);
    }


    private void OnDisable()
    {
        BoardManager.Instance.onGameStateChange.RemoveListener(StateChange);
        BoardManager.Instance.OnPlayerTurnChanged.RemoveListener(PlayerTurn);
    }
    private void PlayerTurn(Player player)
    {
        turnIndicatorText.text = player.ToString();
    }

    private void StateChange(GameState gameState)
    {
        phaseIndicatorText.text = gameState.ToString();
    }


    private void Start()
    {
        player1Text.text = SettingsManager.Instance.player1.name;
        player2Text.text = SettingsManager.Instance.player2.name;
    }
    private void UpdateUI()
    {
        turnIndicatorText.text = $"Current Turn: {BoardManager.Instance.currentPlayer}";
        phaseIndicatorText.text = $"Current Phase: {BoardManager.Instance.currentState}";
    }
}
