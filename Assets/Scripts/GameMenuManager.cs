using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject gameMenuUI;

    private void Start()
    {
        if (gameMenuUI != null)
        {
            gameMenuUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameMenuUI.activeSelf)
            {
                ContinueGame();
            }
            else
            {
                ShowGameMenu();
            }
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene"); 
    }

    public void ExitGame()
    {
        Debug.Log("Game Exiting");
        Application.Quit();

    }

    public void ShowGameMenu()
    {
        if (gameMenuUI != null)
        {
            gameMenuUI.SetActive(true);
        }

    }

    public void ContinueGame()
    {
        gameMenuUI.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

}
