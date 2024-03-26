using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float TimerCount;
    public TMP_Text TimerText;
    public GameObject Timer;
    public GameObject TempGameOver;
    public GameObject GameplayUI;
    public GameObject BackgroundUI;

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TimerCount > 0)
        {
            TimerCount -= Time.deltaTime;
            UpdateTimer(TimerCount);
        }
        else
        {
            Debug.Log("Time is UP!");
            TimerCount = 0;
            UpdateTimer(TimerCount);
            Timer.SetActive(false);
        }
    }

    public void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        float milliseconds = Mathf.FloorToInt((currentTime - Mathf.Floor(currentTime)) * 100); // Extract milliseconds

        TimerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }


    public void GameOver()
    {
        GameplayUI.SetActive(false);
        TempGameOver.SetActive(true);
        BackgroundUI.SetActive(true);
    }
}