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

    [SerializeField] private GameObject winingPanel;
    [SerializeField] private TextMeshProUGUI winingText;

    private void OnEnable()
    {
        BoardManager.Instance.onGameStateChange.AddListener(StateChange);
        BoardManager.Instance.OnPlayerTurnChanged.AddListener(PlayerTurn);
        BoardManager.Instance.OnPlayerWon.AddListener(PlayerWon);
    }


    private void OnDisable()
    {
        BoardManager.Instance.onGameStateChange.RemoveListener(StateChange);
        BoardManager.Instance.OnPlayerTurnChanged.RemoveListener(PlayerTurn);
        BoardManager.Instance.OnPlayerWon.RemoveListener(PlayerWon);
    }
    private void PlayerTurn(Player player)
    {
        turnIndicatorText.text = player.ToString();
    }

    private void StateChange(GameState gameState)
    {

        if(gameState == GameState.PlacementPhase)
        {
            phaseIndicatorText.text = "Place piece";
        }
        else if(gameState == GameState.MovementPhase)
        {
            phaseIndicatorText.text = "Move piece";
        }
        else
        {
            phaseIndicatorText.text = "Remove other player's piece";
        }
    }

    private void PlayerWon(Player player)
    {
        winingPanel.gameObject.SetActive(true);
        winingText.text = $"{player.ToString()} Wins!";
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