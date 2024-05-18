using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public moveCamera camData; // Assumed to be a script containing player data
    public GameObject TempGameOver;
    public GameObject GameplayUI;
    public GameObject BackgroundUI;
    public Slider p1Health;
    public Slider p1KO;
    public Slider p2Health;
    public Slider p2KO;

    void Start()
    {
        // Initialization can be done here if needed
    }

    void FixedUpdate()
    {
        // Check if camData or camData.players is null or if the list is empty
        if (camData?.players == null || camData.players.Count == 0)
        {
            return; // Exit if no players are available
        }

        // Update player health and KO scales safely
        UpdatePlayerStatus();

        // Check each player's health for game over condition
        foreach (var player in camData.players)
        {
            var stateManager = player.GetComponent<PlayerStateManager>();
            if (stateManager != null && stateManager.health < 0)
            {
                GameOver();
                break; // Exit the loop after game over to avoid redundant checks
            }
        }
    }

    void UpdatePlayerStatus()
    {
        if (camData.players.Count > 0 && camData.players[0] != null)
        {
            var p1StateManager = camData.players[0].GetComponent<PlayerStateManager>();
            if (p1StateManager != null)
            {
                p1Health.value = p1StateManager.health;
                p1KO.value = p1StateManager.KOScale;
            }
        }

        if (camData.players.Count > 1 && camData.players[1] != null)
        {
            var p2StateManager = camData.players[1].GetComponent<PlayerStateManager>();
            if (p2StateManager != null)
            {
                p2Health.value = p2StateManager.health;
                p2KO.value = p2StateManager.KOScale;
            }
        }
    }

    public void GameOver()
    {
        GameplayUI.SetActive(false);
        TempGameOver.SetActive(true);
        BackgroundUI.SetActive(true);
    }
}
