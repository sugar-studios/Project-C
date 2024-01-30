using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class titleManager : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject HomeDashboard;
    public GameObject Duels;
    public GameObject Options;
    public GameObject Practice;

    private GameObject _currentScreen;
    private List<GameObject> _screenCollection;

    void Start()
    {
        _screenCollection = new List<GameObject> { TitleScreen, HomeDashboard, Duels, Options, Practice };
        _currentScreen = TitleScreen;

        foreach (var screen in _screenCollection)
        {
            if (screen != null)
            {
                screen.SetActive(false);
            }
        }

        if (_currentScreen != null)
        {
            _currentScreen.SetActive(true);
        }
    }

    public void ScreenTransition(GameObject targetScreen)
    {
        if (_currentScreen != null)
        {
            _currentScreen.SetActive(false);
        }

        if (targetScreen != null)
        {
            targetScreen.SetActive(true);
            _currentScreen = targetScreen;
        }
    }

    public void CoolFunction()
    {
        Debug.Log("CLICK!");
    }
}
