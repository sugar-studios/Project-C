using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
    public PlayerInputReceiver inputReceiver;
    public RectTransform cursorTransform; // UI element representing the cursor
    public Canvas canvas; // Reference to the canvas

    private Vector2 moveInput;
    public bool inMenu;

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
        if (inMenu && isAttacking)
        {
            // Perform the light attack action
            Debug.Log("Light attack performed in menu");
        }
    }

    private void Update()
    {
        if (inMenu)
        {
            cursorTransform.anchoredPosition += moveInput * Time.deltaTime * 100f; // Adjust the multiplier as needed
        }
    }

    public void SetMenuState(bool menuState)
    {
        inMenu = menuState;
        cursorTransform.gameObject.SetActive(inMenu);

        if (inMenu)
        {
            // Recenter the cursor
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            cursorTransform.anchoredPosition = new Vector2(canvasRect.rect.width / 2, canvasRect.rect.height / 2);
        }
    }
}
