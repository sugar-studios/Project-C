using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CustomCursorController : MonoBehaviour
{
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;
    private GameObject lastHover;
    private GameObject lastHoveredButton;

    void Start()
    {
        eventSystem = EventSystem.current;
    }

    void Update()
    {
        CheckUIElementUnderCursor();
    }

    private void CheckUIElementUnderCursor()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = transform.position; // Use the custom cursor's position

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerEventData, results);

        GameObject currentHover = results.Count > 0 ? results[0].gameObject : null;

        if (currentHover != lastHover)
        {
            if (lastHover != null)
            {
                ExecuteEvents.Execute<IPointerExitHandler>(lastHover, pointerEventData, ExecuteEvents.pointerExitHandler);
            }

            if (currentHover != null)
            {
                ExecuteEvents.Execute<IPointerEnterHandler>(currentHover, pointerEventData, ExecuteEvents.pointerEnterHandler);
            }

            lastHover = currentHover;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        ExecuteEvents.Execute<IPointerEnterHandler>(collision.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        lastHoveredButton = collision.gameObject;
    }

    private void OnTriggerExit(Collider collision)
    {
        ExecuteEvents.Execute<IPointerExitHandler>(collision.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
        lastHoveredButton = null;
    }
}
