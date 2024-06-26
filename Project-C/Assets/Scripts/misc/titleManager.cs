using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class titleManager : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject HomeDashboard;
    public GameObject DuelsScreen;
    public GameObject OptionsScreen;
    public GameObject PracticeScreen;
    public GameObject LoadScreen;
    public GameObject DuelsImage;
    public GameObject OptionsImage;
    public GameObject PracticeImage;
    public Button DuelsButton;
    public Button OptionsButton;
    public Button PracticeButton;
    public AudioSource Select;



    private PlayerControls _Controls;
    private Vector2 _Move;
    private GameObject _CurrentScreen;
    private GameObject _CurrentImage;
    private List<GameObject> _ScreenCollection;

    private void Awake()
    {
        _Controls = new PlayerControls();
        _Controls.Gameplay.Start.performed += ctx => StartButton();
        _Controls.Gameplay.Move.performed += ctx => _Move = ctx.ReadValue<Vector2>();
        _Controls.Gameplay.Move.canceled += ctx => _Move = Vector2.zero;

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
        if (_CurrentScreen == TitleScreen) { ScreenTransition(HomeDashboard); Select.Play(); }
    }

    void Start()
    {
        _ScreenCollection = new List<GameObject> { TitleScreen, HomeDashboard, DuelsScreen, OptionsScreen, PracticeScreen, LoadScreen};
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


    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsynchronusly(sceneName));
    }

    IEnumerator LoadSceneAsynchronusly(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            yield return null;
        }
    }
}