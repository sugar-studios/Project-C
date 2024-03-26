using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public void SetSelectedButton(GameObject targetButton)
    {
        if (targetButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(targetButton);
        }
    }
}