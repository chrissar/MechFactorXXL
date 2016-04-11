using UnityEngine;
using System;
using System.Collections;

public class StateTransition
{
    public StateTransition(State next, TransitionCondition condition, Action action)
    {
        NextState = next;
        this.condition = condition;
        transitionAction = action;
        if (condition == null)
        {
            throw new UnityException("Transition to " + next.Name + " has a null condition.");
        }
    }
    public delegate bool TransitionCondition();
    private TransitionCondition condition;
    private Action transitionAction;
    public State NextState
    {
        get; set;
    }
    public bool IsReady()
    {
        return condition();
    }
    public void Activate()
    {
        if (transitionAction != null)
        {
            transitionAction.Invoke();
        }
    }
}
