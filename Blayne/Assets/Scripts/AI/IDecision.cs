using UnityEngine;
using System.Collections;

public interface IDecision
{
    // Common states
    // On...Enter for anything that needs to be initialized once on transition.
    // On...Exit for anything that needs to be uninitialized.
    void UpdateState();
    void OnStateEnter();
    void OnStateExit();
    void OnClearState();


    void ToExecute();
    void ToDeciding();
    void ToIdling();

}
