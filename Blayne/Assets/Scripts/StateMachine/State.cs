using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class State
{
    private List<StateTransition> _transitions;
    public string Name
    {
        get; set;
    }
    private Action stateAction;
    public State(string name, Action action)
    {
        Name = name;
        stateAction = action;
        _transitions = new List<StateTransition>();
    }
    public void AddTransition(StateTransition transition)
    {
        _transitions.Add(transition);
    }
    public State Transition()
    {
        State nextState = this;
        foreach (StateTransition transition in _transitions)
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
        if (stateAction != null)
        {
            stateAction.Invoke();
        }
    }
}
