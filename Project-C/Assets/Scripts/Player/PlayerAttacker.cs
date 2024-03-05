using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerAttacker : MonoBehaviour
{
    public PlayerStateManager PlayerState;

    void Start()
    {
        PlayerState = gameObject.GetComponent<PlayerStateManager>();
    }
    public void NormalAttackL()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("A");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            AttackTimeOut();
        }
    }
    public void NormalAttackM()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("B");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            AttackTimeOut();
        }
    }
    public void NormalAttackH()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("C");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            AttackTimeOut();
        }
    }
    public void SpecialAttack()
    {
        if (PlayerState.State == PlayerStateManager.PossibleStates.FreeAction)
        {
            Debug.Log("D");
            PlayerState.State = PlayerStateManager.PossibleStates.Attacking;
            AttackTimeOut();
        }
    }

    void AttackTimeOut()
    {
        Invoke("RestartState", 0.5f);
    }

    void RestartState()
    {
        PlayerState.State = PlayerStateManager.PossibleStates.FreeAction;
    }
}
