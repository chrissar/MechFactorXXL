using UnityEngine;
using System.Collections;

public interface IUnitAction
{
    // Common states
    // On...Enter for anything that needs to be initialized once on transition.
    // On...Exit for anything that needs to be uninitialized.
    void UpdateState();
    void OnStateEnter();
    void OnStateExit();
    void OnClearState();

    // States that the unit has
    // these states are not necassarily exclusive, meaning that once set
    // to the "crouch" state, going to moving may not uncrouch you.
    // Use OnClearState or OnStateExit when a state's properties need to be cleared.
    void ToIdling();
    void ToMoving();
    void ToSprinting();
    void ToSuppressed();
    void ToAlert();
    void ToFiring();
    void ToInCover();
    void ToAiming();
    void ToReloading();
    void ToCrouching();
    void ToStanding();
    void ToProne();

}
