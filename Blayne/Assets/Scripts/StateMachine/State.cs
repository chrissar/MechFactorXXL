using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class State
{
    private List<StateTransition> mTransitions;
    public string Name
    {
        get; set;
    }
    private Action mStateAction;
    public State(string name, Action action)
    {
        Name = name;
        mStateAction = action;
        mTransitions = new List<StateTransition>();
    }
    public void AddTransition(StateTransition transition)
    {
        mTransitions.Add(transition);
    }
    public State Transition()
    {
        State nextState = this;
        foreach (StateTransition transition in mTransitions)
        {
            if (transition.IsReady())
            {
                transition.Activate();
                nextState = transition.NextState;
                break;
            }
        }
        return nextState;
    }
    public void Act()
    {
        if (mStateAction != null)
        {
            mStateAction.Invoke();
        }
    }
}
