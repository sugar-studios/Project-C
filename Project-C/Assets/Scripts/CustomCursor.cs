using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CustomCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerInputReceiver inputReceiver;
    public RectTransform cursorTransform; // UI element representing the cursor
    public Canvas canvas; // Reference to the canvas
    public RectTransform uiPanelTransform; // Assign the UI panel transform in the inspector

    private Vector2 moveInput;
    public bool inMenu;
    private Button hoveredButton; // The button currently under the cursor

    private void OnEnable()
    {
        inputReceiver.OnMoveEvent += HandleMove;
        inputReceiver.OnLightAttackEvent += HandleLightAttack;
    }

    private void OnDisable()
    {
        inputReceiver.OnMoveEvent -= HandleMove;
        inputReceiver.OnLightAttackEvent -= HandleLightAttack;
    }

    private void Start()
    {
        // Assign the canvas if it is not already assigned
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }

        // Find the UI panel tagged as "selection" and get its RectTransform
        uiPanelTransform = GameObject.FindWithTag("selection")?.GetComponent<RectTransform>();

        if (uiPanelTransform == null)
        {
            Debug.LogError("No GameObject with tag 'selection' found, or it does not have a RectTransform component.");
        }

        AttachToUIPanel();
    }



    private void AttachToUIPanel()
    {
        if (uiPanelTransform != null && cursorTransform != null)
        {
            cursorTransform.SetParent(uiPanelTransform, false);
            cursorTransform.anchoredPosition = Vector2.zero;
        }
        else
        {
            Debug.LogError("UI Panel Transform or Cursor Transform is not assigned.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerEnter != null)
        {
            hoveredButton = eventData.pointerEnter.GetComponent<Button>();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Check if the current gameObject is not the same as the one stored in hoveredButton
        if (hoveredButton != null && hoveredButton.gameObject != eventData.pointerCurrentRaycast.gameObject)
        {
            hoveredButton = null;
        }
    }


    private void HandleMove(Vector2 move)
    {
        if (inMenu)
        {
            moveInput = move;
        }
    }

    private void HandleLightAttack(bool isAttacking)
    {
        if (inMenu && isAttacking && hoveredButton != null)
        {
            hoveredButton.onClick.Invoke();
            Debug.Log("Button clicked via light attack");
        }
    }

    private void Update()
    {
        if (inMenu)
        {
            cursorTransform.anchoredPosition += new Vector2(moveInput.x, moveInput.y) * 100f * Time.deltaTime;


            // Simulate Pointer Events for UI Elements under the cursor
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, cursorTransform.position);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            foreach (var result in results)
            {
                if (result.gameObject.GetComponent<Button>())
                {
                    hoveredButton = result.gameObject.GetComponent<Button>();
                    break;
                }
            }
        }
    }

    public void SetMenuState(bool menuState)
    {
        inMenu = menuState;
        cursorTransform.gameObject.SetActive(inMenu);

        if (inMenu)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            cursorTransform.anchoredPosition = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
        }
    }
}
