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

    private GameObject _CurrentScreen;
    private List<GameObject> _ScreenCollection;

    void Start()
    {
        _ScreenCollection = new List<GameObject> { TitleScreen, HomeDashboard, Duels, Options, Practice };
        _CurrentScreen = TitleScreen;

        foreach (var screen in _ScreenCollection)
        {
            if (screen != null)
            {
                screen.SetActive(false);
            }
        }

        if (_CurrentScreen != null)
        {
            _CurrentScreen.SetActive(true);
        }
    }

    public void ScreenTransition(GameObject targetScreen)
    {
        if (_CurrentScreen != null)
        {
            _CurrentScreen.SetActive(false);
        }

        if (targetScreen != null)
        {
            targetScreen.SetActive(true);
            _CurrentScreen = targetScreen;
        }
    }

    public void CoolFunction()
    {
        Debug.Log("CLICK!");
    }

    private void Update()
    {
        if (_CurrentScreen == TitleScreen)
        {
            Debug.Log("heelo");
        }
    }
}
