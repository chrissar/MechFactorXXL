using UnityEngine;
using System.Collections;

public interface IMovement
{
    // Common states
    // On...Enter for anything that needs to be initialized once on transition.
    // On...Exit for anything that needs to be uninitialized.
    void UpdateState();
    void OnStateEnter();
    void OnStateExit();

    void ToMoving();
    void ToIdling();
    void ToSprinting();

}
