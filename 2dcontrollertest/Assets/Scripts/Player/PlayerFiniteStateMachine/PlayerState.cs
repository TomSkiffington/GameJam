using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected Core core;

    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected PlayerData playerData;

    protected bool isAnimationFinished;
    protected bool isExitingState;

    protected float startTime;      //how long been in state

    private string animBoolName;    //state name for animator


    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
        core = player.Core;
    }

    public virtual void Enter() {   //called on state enter
        DoChecks();
        player.Anim.SetBool(animBoolName, true);
        startTime = Time.time;
        //Debug.Log(animBoolName);
        isAnimationFinished = false;
        isExitingState = false;
        
    }

    public virtual void Exit() {    //called on state exit
        player.Anim.SetBool(animBoolName, false);
        isExitingState = true;
    }

    public virtual void LogicUpdate() {     //called every frame

    }

    public virtual void PhysicsUpdate() {       //called every fixed update
        DoChecks();
    }

    public virtual void DoChecks() {

    }

    public virtual void AnimationTrigger() {

    }

    public virtual void AnimationFinishTrigger() {
        isAnimationFinished = true;
    }

}
