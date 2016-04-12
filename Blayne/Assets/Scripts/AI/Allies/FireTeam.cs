using System;
using System.Collections.Generic;
using UnityEngine;

public enum FireTeamFormation {WEDGE, FILE, COVER};

public class FireTeam : Ally
{

	public const int kMaxFireTeamMembers = 4;
	private const float mkMinDistanceFromSlotPositionNeeded = 5.0f;
	private const float mkOptimalAttackDistance = FireTeamAlly.kVisionConeRadius * 0.5f;

    public enum Side
    {
        Friend,
        Enemy
    }
    public int teamNumber;
	public List<FireTeam> alliedFireTeams;
	public Vector3 spawnPoint;
    private Side mSide;

	private Projector mProjector;
	private FireTeamFormation mCurrentFireTeamFormation;
	private Vector3 mDestination;
	private int mMemberCount;
	private FireTeamAlly[] mFireTeamMembers;
	private Vector3[] mRelativeSlotDisplacements;
	private List<FireTeamAlly> mDisabledTeamMembers;
	private List<FireTeam> mEngagedEnemyTeams;
	private FireTeam mEnemyTeamToAttack;
	private Vector3 mCurrentAnchorPosition;
	private Vector3 mNextAnchorPosition; // Slightly ahead of anchor point to set target slot positions.
	private Quaternion mCurrentOrientation;
	private float mCurrentSpeed;

    private State mCurrentState;
	public Side TeamSide
	{
		get
		{
			return mSide;
		}
		set 
		{
			mSide = value;
			// Disable the projector if the team is an enemy team.
			if (mProjector != null && mSide == Side.Enemy) {
				mProjector.enabled = false;
			} else {
				mProjector.enabled = true;
			}
		}
	}
    public List<FireTeamAlly> GetAllMembers()
    {
        List<FireTeamAlly> result = new List<FireTeamAlly>();
        result.AddRange(mFireTeamMembers);
        return result;
    }
	public Vector3 Destination
	{
		get
		{
			return mDestination;
		}
	}
	public FireTeamFormation CurrentFireTeamFormation
	{
		get
		{ 
			return mCurrentFireTeamFormation; 
		}
	}
	public  List<FireTeam> EngagedEnemyTeams
	{
		get
		{ 
			return mEngagedEnemyTeams;
		}
	}
	public  FireTeam EnemyTeamToAttack
	{
		get
		{ 
			return mEnemyTeamToAttack;
		}
		set 
		{	
			FireTeam fireTeamToAttack = value;
			// Only set the enemy team if it is on the opposite team.
			if (fireTeamToAttack != null && fireTeamToAttack.mSide != mSide) {
				mEnemyTeamToAttack = fireTeamToAttack;
			} 
		}
	}
	public int MemberCount
	{
		get
		{ 
			return mMemberCount;
		}
	}
	public Vector3 CurrentAnchorPosition
	{
		get
		{ 
			return mCurrentAnchorPosition;
		}
		set
		{ 
			mCurrentAnchorPosition = value;
			// Reset the next anchor position as well so that the slot positions returned 
			// by GetSlotPosition() reflect the new current anchor position without an extra
			// update frame needing to be called.
			mNextAnchorPosition = mCurrentAnchorPosition;
		}
	}

	public void Awake()
	{
		Initialize ();
        InitializeStateMachine();
	}

	public void Update()
	{
		UpdateDestination ();
		UpdateProjector ();
		UpdateAnchor ();
	}

    private void InitializeStateMachine()
    {
        State attackState = new State("Attack", () =>
        {
            // Need to call attack function for squad
            Debug.Log("Attacking");
        });
        State coverState = new State("Cover", () =>
        {
            // Need to call cover function for squad
            Debug.Log("Attacking");
        });
        State retreatState = new State("Right Cast", () =>
        {
            // Need to call retreat function for squad
            Debug.Log("Attacking");
        });

        // Initial state
        mCurrentState = attackState;

        //State transition definitions
        attackState.AddTransition(new StateTransition(coverState, () =>
        {
            // Add condition for transitioning to cover from attacking. Stubbed to return false for now
            return false;
        }, () =>
        {
            Debug.Log("Go to cover");
        }));

        attackState.AddTransition(new StateTransition(retreatState, () =>
        {
            // Add condition for transitioning to retreating from attacking
            return mDisabledTeamMembers.Count > 3;
        }, () =>
        {
            new TeamDisengageCommand().execute(this);
        }));

        //Left Cast State transitions
        coverState.AddTransition(new StateTransition(attackState, () =>
        {
            // Add condition for transitioning to attacking from cover. Stubbed to be false. Ideally will need to know if cover exists.
            return false;
        }, () =>
        {
            new TeamAttackEnemyCommand(mEngagedEnemyTeams[UnityEngine.Random.Range(0, mEngagedEnemyTeams.Count)]).execute(this);
        }));
        coverState.AddTransition(new StateTransition(retreatState, () =>
        {
            // Add condition for transitioning to retreating from cover.
            return mDisabledTeamMembers.Count > 3;
        }, () =>
        {
            new TeamDisengageCommand().execute(this);
        }));

        //Right Cast State transitions
        retreatState.AddTransition(new StateTransition(attackState, () =>
        {
            // Add condition for transitioning to attacking from retreating
            return false;
        }, () =>
        {
            new TeamAttackEnemyCommand(mEngagedEnemyTeams[UnityEngine.Random.Range(0, mEngagedEnemyTeams.Count)]).execute(this);
        }));
        retreatState.AddTransition(new StateTransition(coverState, () =>
        {
            // Add condition for transitioning to cover from retreating
            return false;
        }, () =>
        {
            Debug.Log("Go to cover");
        }));

    }

	public FireTeamAlly GetAllyAtSlotPosition(int slotPosition)
	{
		if (slotPosition >= 0 && slotPosition < mFireTeamMembers.Length) {
			return mFireTeamMembers [slotPosition];
		}
		return null;
	}

	public void SetDestination(Vector3 destination)
	{
		mDestination = destination;
		SetNextAnchorPointTarget ();
		SetOrientation ();
	}

	public void SetFormation(FireTeamFormation newFireTeamFormation)
	{
		mCurrentFireTeamFormation = newFireTeamFormation;
		switch (mCurrentFireTeamFormation) {
			case FireTeamFormation.WEDGE:
				// Set the slot positions for the wedge formation.
				SetWedgeSlotPositions ();
				mCurrentFireTeamFormation = FireTeamFormation.WEDGE;
				break;
			case FireTeamFormation.FILE:
				// Set the slot positions for the file formation.
				SetFileSlotPositions ();
				mCurrentFireTeamFormation = FireTeamFormation.FILE;
				break;
			case FireTeamFormation.COVER:
				// Set the slot positions for the taking of cover formation.
				SetCoverSlotPositions ();
				mCurrentFireTeamFormation = FireTeamFormation.COVER;
				break;
		}
		// Update the formation to take into account the new formation setup.
		UpdateFormation ();
	}
		
	public void AddFireTeamAlly(FireTeamAlly fireTeamAllyToAdd)
	{
		if (fireTeamAllyToAdd == null) {
			return;
		}
		// Add aly to the list of allies.
		if(mMemberCount < mFireTeamMembers.Length){
			// Add team member to list of non-leader team members if there is room.
			mFireTeamMembers[mMemberCount] = fireTeamAllyToAdd;
			mMemberCount++;
		}
		// Set fire team ally's team to this team.
		fireTeamAllyToAdd.fireTeamNumber = teamNumber;
		fireTeamAllyToAdd.fireTeam = this;
		// Update the formation to account for the added member.
		UpdateFormation();
	}

	public void EnableFireTeamAlly(FireTeamAlly fireTeamAllyToEnable)
	{
		// Make sure the ally to re-add was once part of the squad by checking
		// if the ally is in the list of disabled team members.
		if (fireTeamAllyToEnable == null || 
			mDisabledTeamMembers.Contains (fireTeamAllyToEnable) == false) {
			return;
		}
		// Check if there is currently a leader.
		// Only allow enabling of allies if the team is not full while there is a leader.
		if (mMemberCount >= mFireTeamMembers.Length) {
			return;
		}
		// Make sure the fire team ally has a valid slot position. Note that 
		// slot positions for non-leader team members have an offset of 1
		// from their place in the non-leader team member list.
		if (fireTeamAllyToEnable.slotPosition < 0 ||
		    fireTeamAllyToEnable.slotPosition > mFireTeamMembers.Length) {
			return;
		}

		// Insert the enabled ally back into its original slot position.
		InsertTeamMemberAtIndex (fireTeamAllyToEnable, fireTeamAllyToEnable.slotPosition);

		// Set fire team ally's team to this team.
		fireTeamAllyToEnable.fireTeam = this;
		fireTeamAllyToEnable.IsDisabled = false; // Enable the ally.
		// Update the formation to account for the enabled member.
		UpdateFormation();
		// Remove the ally from the list of disabled allies.
		mDisabledTeamMembers.Remove(fireTeamAllyToEnable);
	}

	// Returns the ally that replaces the removed ally, including a replaced leader, but 
	// can return null if there were no replacement ally.
	public FireTeamAlly RemoveFireTeamAlly(FireTeamAlly fireTeamAllyToRemove)
	{
		if (fireTeamAllyToRemove == null) {
			return null;
		}
		// Remove the ally, and get the ally that replaced the removed one.
		FireTeamAlly promotedFireTeamAlly = RemoveFireTeamAllyFromTeamMemberList(fireTeamAllyToRemove);
		// Update the formation to account for the removed member.
		UpdateFormation();
		// Return the ally that replaced the removed one.
		return promotedFireTeamAlly;
	}

	// Returns the ally that replaces the disabledally, including a disabled leader, but 
	// can return null if there were no replacement ally.
	public FireTeamAlly DisableFireTeamAlly(FireTeamAlly fireTeamAllyToDisable)
	{
		if (fireTeamAllyToDisable == null) {
			return null;
		}
	

		// Remove the ally, and get the ally that replaced the removed one.
		FireTeamAlly promotedFireTeamAlly = RemoveFireTeamAllyFromTeamMemberList(fireTeamAllyToDisable);
		// Update the formation to account for the removed member.
		UpdateFormation();
		// Insert the disabled ally into the list of disabled allies.
		mDisabledTeamMembers.Add(fireTeamAllyToDisable);
		// Disable the ally.
		fireTeamAllyToDisable.IsDisabled = true; 
		// Return the ally that replaced the removed one.
		return promotedFireTeamAlly;
	}
		
	public Vector3 GetSlotPosition(int slotNumber)
	{
		if (0 <= slotNumber && slotNumber < mRelativeSlotDisplacements.Length){
			// If the fire team is taking cover (which is urgent) use the destination for the 
			// slot position displacement, taking into account the orientation of the team.
			// Otherisez use the next anchor position for the displacement.
			if (mCurrentFireTeamFormation == FireTeamFormation.COVER) {
				return mDestination + (mCurrentOrientation * mRelativeSlotDisplacements [slotNumber]);
			}
			return mNextAnchorPosition + (mCurrentOrientation * mRelativeSlotDisplacements [slotNumber]);
		}
		return Vector3.zero;
	}

	public bool IsFireTeamInPosition()
	{
		// Check if each fire team member is close enough to their slot position. 
		// If they are not return false.
		for (int i = 0; i < mMemberCount; ++i) {
			FireTeamAlly fireTeamAlly = mFireTeamMembers [i];
			if(IsFireTeamAllyAtSlotInPosition(i) == false)
			{
				return false;
			}
		}

		// If all fire team members are close enough to their slot positions, return true;
		return true;
	}

	public bool IsFireTeamAllyAtSlotInPosition(int slotPosition){
		FireTeamAlly fireTeamAlly = GetAllyAtSlotPosition (slotPosition);
		// A non-existent ally is considered to be in position.
		if (fireTeamAlly == null) {
			return true;
		}
		// Check if the ally is close enough to the ally's current target.
		// If the ally is detached, check if they are close enough to their detached postion. Otherwise,
		// check if the ally is close enough to the assigned slot position for that ally.
		Vector3 allyTarget = Vector3.zero;
		if (fireTeamAlly.IsDetached) {
			allyTarget = fireTeamAlly.DetachDestination;
		} else {
			allyTarget = GetSlotPosition (fireTeamAlly.slotPosition);
		}
		if (Vector3.Distance (fireTeamAlly.Position, allyTarget) < mkMinDistanceFromSlotPositionNeeded) {
			return true;
		}
		return false;
	}

	protected void Initialize ()
	{
		teamNumber= 0;
		alliedFireTeams = new List<FireTeam> ();
		mProjector = gameObject.GetComponentInChildren<Projector>();
		mDestination = Vector3.zero;
		mMemberCount = 0;
		mFireTeamMembers = new FireTeamAlly[kMaxFireTeamMembers];
		mRelativeSlotDisplacements = new Vector3[kMaxFireTeamMembers];
		mDisabledTeamMembers = new List<FireTeamAlly> ();
		mEngagedEnemyTeams = new List<FireTeam> ();
		mEnemyTeamToAttack = null;
		mCurrentAnchorPosition = Vector3.zero;
		gameObject.transform.position = mCurrentAnchorPosition;
		mNextAnchorPosition = mCurrentAnchorPosition;
		mCurrentFireTeamFormation = FireTeamFormation.WEDGE;
		SetWedgeSlotPositions ();
		mCurrentOrientation = Quaternion.identity;
		mCurrentSpeed = 3.0f;
	}

	private void UpdateDestination()
	{
		// If there is an enemy that the team is currently attacking, set the destination 
		// of the fire team to the optimal attacking distance from the enemy team.
		if (mEnemyTeamToAttack != null) {
			// Move to optimal attack distance along the line between the enemy and the
			// current anchor position.
			Vector3 targetDirection = mCurrentAnchorPosition - mEnemyTeamToAttack.mCurrentAnchorPosition;
			targetDirection.Normalize();
			// To prevent rounding errors, only set the new destination if it is 
			// sufficiently far from the current destination.
			Vector3 newDestination =  mEnemyTeamToAttack.mCurrentAnchorPosition +
				targetDirection * mkOptimalAttackDistance;
			if (Vector3.Distance (newDestination, mDestination) > 2.0f) {
				mDestination = newDestination;
			}
		}
	}

	private void UpdateFormation()
	{
		// Set the anchor point based on the current members of the team.
		SetAnchorPoint();
		// Assign the slot positions of the current members in the team.
		AssignSlotPositions();
	}
		
	private void UpdateProjector()
	{
		if (mMemberCount > 0) {
			mProjector.enabled = true;
			// Set the projector to the center of mass of all the members in the fire team.
			Vector3 positionSum = Vector3.zero;
			for (int i = 0; i < mMemberCount; ++i) {
				positionSum += mFireTeamMembers [i].Position; // Add the fire team member positions.
			}
			gameObject.transform.position = positionSum / (mMemberCount); // include leader position.
		} else {
			mProjector.enabled = false;
		}
	}

	private void UpdateAnchor()
	{
		// Only move the anchor if the fire team is close enough to their slot positions
		// pr if the team is taking cover (which is urgent).
		if (IsFireTeamInPosition () || mCurrentFireTeamFormation == FireTeamFormation.COVER) {
			// Update current anchor point to move towards destination based on overall team speed.
			Vector3 destinationDisplacement = mDestination - mCurrentAnchorPosition;
			// To prevent rounding errors, only update the current anchor position if the 
			// destination is sufficiently far from it.
			if (destinationDisplacement.magnitude > 2.0f) {
				mCurrentAnchorPosition += 
					destinationDisplacement.normalized * mCurrentSpeed * Time.deltaTime;
			} 
		}
		SetNextAnchorPointTarget ();
	}

	private void SetAnchorPoint()
	{
		// Get the average position of all the current fire team members to set as the anchor point.
		Vector3 positionSum = Vector3.zero;
		for (int i = 0; i < mMemberCount; ++i) {
			positionSum += mFireTeamMembers[i].Position; // Add the fire team member positions.
		}
		mCurrentAnchorPosition = positionSum / (mMemberCount); // include leader position.
	}

	private void AssignSlotPositions()
	{
		// Assign each slot to the member in the fire team according to 
		// their position in the list of team members.
		for (int i = 0; i < mMemberCount; ++i) {
			mFireTeamMembers [i].slotPosition = i; 
		}
	}

	private void SetNextAnchorPointTarget()
	{
		// Set future anchor position to be between the current anchor position and the 
		// destination by a certain amount (2x max speed of team) from the current 
		// anchor position.
		Vector3 destinationDisplacement = mDestination - mCurrentAnchorPosition;
		float destinationDistance = destinationDisplacement.magnitude;
		if (destinationDistance < 2 * mCurrentSpeed) {
			mNextAnchorPosition = mDestination;
		} else {
			// Set the next anchor position to the point at the maximum next anchor distance from
			// the current anchor position, along the direction to the destination.
			mNextAnchorPosition = 
				mCurrentAnchorPosition + destinationDisplacement.normalized * 2 * mCurrentSpeed;
		}
	}

	private void SetOrientation()
	{
		// Set orientation to the direction that faces the destination.
		Vector3 destinationDisplacement = mDestination - mCurrentAnchorPosition;
		// Only change orientation if the destination is sufficiently far away to 
		// avoid the orientation of the team from flipping on the spot due to 
		// usage of extremely small vectors.
		if (destinationDisplacement.magnitude > 1.0f) {
			mCurrentOrientation = Quaternion.identity;
			mCurrentOrientation.SetFromToRotation (Vector3.right, destinationDisplacement);
		}
	}

	// Returns the ally that replaces the removed ally in the team ordering.
	private FireTeamAlly RemoveFireTeamAllyFromTeamMemberList(FireTeamAlly fireTeamAllyToRemove)
	{
		// Remove the ally from the list of team members.
		// Try to find the ally in the list of non-leader team members.
		int indexOfTeamMemberToRemove = -1;
		for (int i = 0; i < mMemberCount; i++) {
			if (fireTeamAllyToRemove == mFireTeamMembers [i]) {
				indexOfTeamMemberToRemove = i;
				break;
			}
		}
		// Remove the found team member and shift the other team members.
		if (indexOfTeamMemberToRemove > -1 && 
			indexOfTeamMemberToRemove < mFireTeamMembers.Length) {
			RemoveTeamMemberAtIndex (indexOfTeamMemberToRemove);
			// Return the new ally at the index of the removed ally.
			return 	mFireTeamMembers [indexOfTeamMemberToRemove];
		}
		return null;
	}

	private void InsertTeamMemberAtIndex(FireTeamAlly fireTeamAllyToInsert, int index)
	{
		// If the index is already being used, shift team members to make room for 
		// the team member to add without them becoming overwritten themselves.
		if (mFireTeamMembers [index] != null) {
			for (int i = mMemberCount; i > index; --i) {
				mFireTeamMembers [i] = mFireTeamMembers [i - 1];
			}
			// Insert the fire team member into the specified index.
			mFireTeamMembers [index] = fireTeamAllyToInsert;

		} else {
			// Fill the first empty slot found in the list of non-leader
			// team members. This ensures there are no slot gaps in 
			// the formation.
			for (int i = 0; i < mFireTeamMembers.Length; ++i) {
				if (mFireTeamMembers [i] == null) {
					mFireTeamMembers [i] = fireTeamAllyToInsert;
					break;
				}
			}
		}
		// Increment the non-leader team member counter
		++mMemberCount;
	}

	private void RemoveTeamMemberAtIndex(int index)
	{
		// Remove the team member at the specified index and shift the other team members 
		// by shifting the other team members by one position, overwriting the removed 
		// team member.
		for (int i = index; i < (mMemberCount - 1); ++i) {
			mFireTeamMembers [i] = mFireTeamMembers [i + 1];
		}
		// Null the value at the index of the last fire team member shifted.
		mFireTeamMembers [mMemberCount - 1] = null;
		// Decrement the non-leader team member counter
		--mMemberCount;
	}
		
	private void SetWedgeSlotPositions()
	{
		// Set the relative displacements of each of the slots from the anchor point.
		mRelativeSlotDisplacements[0] = new Vector3(0, 0, 0);
		mRelativeSlotDisplacements [1] = new Vector3 (-5.0f, 0, -5.0f);
		mRelativeSlotDisplacements [2] = new Vector3 (-5.0f, 0, 5.0f);
		mRelativeSlotDisplacements [3] = new Vector3 (-10.0f, 0, 10.0f);
	}

	private void SetFileSlotPositions()
	{
		// Set the relative displacements of each of the slots from the anchor point.
		mRelativeSlotDisplacements[0] = new Vector3(0, 0, 0);
		mRelativeSlotDisplacements [1] = new Vector3 (-5.0f, 0, 0);
		mRelativeSlotDisplacements [2] = new Vector3 (-10.0f, 0, 0);
		mRelativeSlotDisplacements [3] = new Vector3 (-15.0f, 0, 0);
	}

	private void SetCoverSlotPositions()
	{
		// Set the relative displacements of each of the slots from the anchor point.
		mRelativeSlotDisplacements[0] = new Vector3(1.5f, 0, 1.5f);
		mRelativeSlotDisplacements [1] = new Vector3 (-1.5f, 0, 1.5f);
		mRelativeSlotDisplacements [2] = new Vector3 (1.5f, 0, -1.5f);
		mRelativeSlotDisplacements [3] = new Vector3 (-1.5f, 0, -1.5f);
	}
}