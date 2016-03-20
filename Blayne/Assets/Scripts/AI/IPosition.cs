using UnityEngine;
using System.Collections;

public interface IPosition
{
    // Common states
    // On...Enter for anything that needs to be initialized once on transition.
    // On...Exit for anything that needs to be uninitialized.
    void UpdateState();
    void OnStateEnter();
    void OnStateExit();

    void ToCrouched();
    void ToStanding();
    void ToProne();
}
