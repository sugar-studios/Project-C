using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Make sure to include this for event handling

// Ensure the class implements the necessary interfaces for pointer events
public class PreviewDashboardImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject DuelsImage;
    public GameObject OptionsImage;
    public GameObject PracticeImage;
    public AudioSource Select1;
    public AudioSource Select2;
    public AudioSource Selcted;

    private void Start()
    {
        // Initially, make all images inactive
        DuelsImage.SetActive(false);
        PracticeImage.SetActive(false);
        OptionsImage.SetActive(false);
    }

    // Implement the OnPointerEnter method from the IPointerEnterHandler interface
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse Enter");
        // Activate corresponding image based on the game object's name
        if (gameObject.name == "Duels")
        {
            DuelsImage.SetActive(true);
            PracticeImage.SetActive(false);
            OptionsImage.SetActive(false);
            ClickSound();
        }
        else if (gameObject.name == "Practice")
        {
            DuelsImage.SetActive(false);
            PracticeImage.SetActive(true);
            OptionsImage.SetActive(false);
            ClickSound();
        }
        else if (gameObject.name == "Options") // Make sure the name matches exactly, case-sensitive
        {
            DuelsImage.SetActive(false);
            PracticeImage.SetActive(false);
            OptionsImage.SetActive(true);
            ClickSound();
        }
    }

    // Implement the OnPointerExit method to handle when the mouse leaves the UI element
    public void OnPointerExit(PointerEventData eventData)
    {
        // Optionally, deactivate all images when the pointer exits the button
        DuelsImage.SetActive(false);
        PracticeImage.SetActive(false);
        OptionsImage.SetActive(false);
    }

    void OnClicked() { 
        Selcted.Play();
    }

    void ClickSound()
    {
        if (UnityEngine.Random.Range(0f, 1f) == 1)
        {
            Select1.Play();
        } else
        {
            Select2.Play();
        }
        
    }
}
