using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public PossibleStates State;

    // Expanded the states to include detailed attack phases and a generic recovering state
    public enum PossibleStates
    {
        HitStun,         // The player is stunned due to taking damage.
        Inactionable,    // The player is temporarily unable to act, possibly due to certain attacks or effects.
        Stunned,         // The player is immobilized, perhaps by an enemy's attack or a game mechanic.
        PsedeuFree,      // The player is nearly free to act but may have slight restrictions.
        FreeAction,      // The player is completely free to move and act.
        PreparingAttack, // The player is in the startup phase of an attack.
        Attacking,       // The player is actively attacking.
        Recovering,      // The player is in the recovery phase after an attack.
        MaintainMomentum // The player maintains momentum while performing certain actions.
    }

    void Start()
    {
        State = PossibleStates.FreeAction; // Setting initial state to FreeAction for maximum responsiveness at start.
    }
}
