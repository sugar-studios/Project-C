using UnityEngine;
using UnityEngine.EventSystems;

public class PreviewDashboardImage : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameObject DuelsImage;
    public GameObject OptionsImage;
    public GameObject PracticeImage;
    public AudioSource Select1;
    public AudioSource Select2;
    public AudioSource Selected;

    private void Start()
    {
        SetActiveImage(null); // Deactivate all images initially
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("Item Selected");
        SetActiveImage(gameObject.name);
        PlayClickSound();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("Item Deselected");
        SetActiveImage(null); // Deactivate all images
    }

    private void SetActiveImage(string imageName)
    {
        DuelsImage.SetActive(imageName == "Duels");
        PracticeImage.SetActive(imageName == "Practice");
        OptionsImage.SetActive(imageName == "Options");
    }

    private void PlayClickSound()
    {
        // Improved logic for playing a random sound
        if (UnityEngine.Random.Range(0, 2) == 0) // Use integer range for a 50/50 chance
        {
            Select1.Play();
        }
        else
        {
            Select2.Play();
        }
    }

    void OnClicked()
    {
        Selected.Play();
    }
}
