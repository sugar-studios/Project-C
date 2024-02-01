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
    public GameObject DuelsImage;
    public GameObject OptionsImage;
    public GameObject PracticeImage;


    private PlayerControls _Controls;
    private GameObject _CurrentScreen;
    private List<GameObject> _ScreenCollection;

    private void Awake()
    {
        _Controls = new PlayerControls();
        _Controls.Gameplay.Start.performed += ctx => StartButton();

    }

    private void OnEnable()
    {
        _Controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _Controls.Gameplay.Disable();
    }

    private void StartButton()
    { 
        if (_CurrentScreen == TitleScreen) { ScreenTransition(HomeDashboard); }
    }

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

    public void PreviewImage(GameObject)


    private void Update()
    {

    }
}
