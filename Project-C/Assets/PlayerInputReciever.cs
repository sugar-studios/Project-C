using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    // Declare events for each action
    public event Action<Vector2> OnMoveEvent;
    public event Action<bool> OnJumpEvent;
    public event Action<bool> OnDashEvent;
    public event Action<bool> OnFastFallEvent;
    public event Action<bool> OnLightAttackEvent;
    public event Action<bool> OnHeavyAttackEvent;
    public event Action<bool> OnTrademarkAttackEvent;
    public event Action<bool> OnPowerStrikeEvent;

    // Input states
    public Vector2 PlayerDirectionalInput;
    public bool Jumped;
    public bool Dash;
    public bool FastFall;
    public bool L_Attack;
    public bool H_Attack;
    public bool T_Attack;
    public bool P_Attack;

    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerDirectionalInput = context.ReadValue<Vector2>();
        OnMoveEvent?.Invoke(PlayerDirectionalInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jumped = context.action.triggered;
        OnJumpEvent?.Invoke(Jumped);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        Dash = context.action.triggered;
        OnDashEvent?.Invoke(Dash);
    }

    public void OnFastFall(InputAction.CallbackContext context)
    {
        FastFall = context.action.triggered;
        OnFastFallEvent?.Invoke(FastFall);
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        L_Attack = context.action.triggered;
        OnLightAttackEvent?.Invoke(L_Attack);
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        H_Attack = context.action.triggered;
        OnHeavyAttackEvent?.Invoke(H_Attack);
    }

    /*
    public void OnTrademarkAttack(InputAction.CallbackContext context)
    {
        T_Attack = context.action.triggered;
        OnTrademarkAttackEvent?.Invoke(T_Attack);
    }
    */

    public void OnPowerStrike(InputAction.CallbackContext context)
    {
        P_Attack = context.action.triggered;
        OnPowerStrikeEvent?.Invoke(P_Attack);
    }
}
