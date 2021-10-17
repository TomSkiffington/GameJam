using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState CurrentState { get; private set; }   //any script with reference to this PlayerState can read this variable, but it can only be set in this script

    public void Initialize(PlayerState startingState) {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(PlayerState newState) {     //exits current state then sets new current state and runs enter()
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
