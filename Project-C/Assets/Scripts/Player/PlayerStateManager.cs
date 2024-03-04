using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public PossibleStates State;

    public enum PossibleStates
    {
        HitStun,
        Inactionable,
        Stunned,
        PsedeuFree,
        FreeAction
    }

    void Start()
    {
        State = PossibleStates.FreeAction;
    }
}
