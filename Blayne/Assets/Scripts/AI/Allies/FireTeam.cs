using System;
using System.Collections.Generic;
using UnityEngine;

public enum FireTeamFormation {WEDGE, FILE, COVER};

public class FireTeam
{

	public const int kMaxFireTeamMembers = 4;
	private const float mkMinDistanceFromSlotPositionNeeded = 5.0f;

	public int teamNumber;

	private FireTeamFormation mCurrentFireTeamFormation;
	private Vector3 mDestination;
	private FireTeamAlly mFireTeamLeader;
	private int mNonLeaderMemberCount;
	private FireTeamAlly[] mFireTeamNonLeaderMembers;
	private Vector3[] mRelativeSlotDisplacements;
	private List<FireTeamAlly> mDisabledTeamMembers;
	private Vector3 mCurrentAnchorPosition;
	private Vector3 mNextAnchorPosition; // Slightly ahead of anchor point to set target slot positions.
	private Quaternion mCurrentOrientation;
	private float mCurrentSpeed;

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
	public int NonLeaderMemberCount
	{
		get
		{ 
			return mNonLeaderMemberCount;
		}
	}
	public Vector3 CurrentAnchorPosition
	{
		get
		{ 
			return mCurrentAnchorPosition;
		}
	}

	public FireTeam ()
	{
		teamNumber = 0;
		mDestination = Vector3.zero;
		mCurrentFireTeamFormation = FireTeamFormation.WEDGE;
		mFireTeamLeader = null;
		mNonLeaderMemberCount = 0;
		mFireTeamNonLeaderMembers = new FireTeamAlly[kMaxFireTeamMembers - 1];
		mRelativeSlotDisplacements = new Vector3[kMaxFireTeamMembers];
		mDisabledTeamMembers = new List<FireTeamAlly> ();
		mCurrentAnchorPosition = Vector3.zero;
		mNextAnchorPosition = mCurrentAnchorPosition;
		mCurrentOrientation = Quaternion.identity;
		mCurrentSpeed = 2.0f;
	}

	public FireTeamAlly GetAllyAtSlotPosition(int slotPosition)
	{
		if (slotPosition == 0) {
			return mFireTeamLeader;
		} else if (slotPosition - 1 < mFireTeamNonLeaderMembers.Length) {
			// Slots of non-leader team members are offset by 1 from their
			// position in the non-leader team list.
			return mFireTeamNonLeaderMembers [slotPosition - 1];
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
		// Set the anchor point of the formation.
		SetAnchorPoint ();
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
		// Assign the slot positions to the members of the fire team.
		AssignSlotPositions ();
	}

	public void UpdateAnchor()
	{
		// Only move the anchor if the fire team is close enough to their slot positions.
		if (IsFireTeamInPosition ()) {
			// Update current anchor point to move towards destination based on overall team speed.
			Vector3 destinationDisplacement = mDestination - mCurrentAnchorPosition;
			mCurrentAnchorPosition += destinationDisplacement.normalized * mCurrentSpeed * Time.deltaTime;
			SetNextAnchorPointTarget ();
		}
	}

	public void AddFireTeamAlly(FireTeamAlly fireTeamAllyToAdd)
	{
		if (fireTeamAllyToAdd == null) {
			return;
		}
		// If the added ally is a leader, set it as the leader if there is currently no leader.
		// Otherwise, add it to the list of non-leaders.
		if (mFireTeamLeader == null && fireTeamAllyToAdd.fireTeamRole == FireTeamRole.LEADER) {
			mFireTeamLeader = fireTeamAllyToAdd;
		} else if(mNonLeaderMemberCount < mFireTeamNonLeaderMembers.Length){
			// Add team member to list of non-leader team members if there is room.
			mFireTeamNonLeaderMembers[mNonLeaderMemberCount] = fireTeamAllyToAdd;
			mNonLeaderMemberCount++;
		}
		// Set fire team ally's team to this team.
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
		if (mFireTeamLeader != null) {
			// Only allow enabling of allies if the team is not full while there is a leader.
			if (mNonLeaderMemberCount >= mFireTeamNonLeaderMembers.Length) {
				return;
			}
			// Make sure the fire team ally has a valid slot position. Note that 
			// slot positions for non-leader team members have an offset of 1
			// from their place in the non-leader team member list.
			if (fireTeamAllyToEnable.slotPosition < 0 ||
			    fireTeamAllyToEnable.slotPosition > mFireTeamNonLeaderMembers.Length) {
				return;
			}
			// If the team member to enable was previously the leader, then reset the 
			// enabled ally to be the leader and insert the current leader back into the list 
			// of non leaders.
			if (fireTeamAllyToEnable.fireTeamRole == FireTeamRole.LEADER) {
				InsertTeamMemberAtIndex (mFireTeamLeader, 0);
				mFireTeamLeader = fireTeamAllyToEnable;
			} else {
				// Set the slot position of the ally to enable to 1 if it is currently at 0
				// (which can happen if the ally was a temporary leader).
				if (fireTeamAllyToEnable.slotPosition == 0) {
					fireTeamAllyToEnable.slotPosition = 1;
				}
				// Insert the enabled ally back into its original slot position.
				InsertTeamMemberAtIndex (fireTeamAllyToEnable, fireTeamAllyToEnable.slotPosition - 1);
			}
		} else {
			// Make the enabled ally the current leader.
			mFireTeamLeader = fireTeamAllyToEnable;
		}
		// Set fire team ally's team to this team.
		fireTeamAllyToEnable.fireTeam = this;
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
		// Clear the fire team ally's current fire team.
		fireTeamAllyToRemove.fireTeam = null;
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
		// Clear the fire team ally's current fire team.
		fireTeamAllyToDisable.fireTeam = null;
		// Return the ally that replaced the removed one.
		return promotedFireTeamAlly;
	}
		
	public Vector3 getSlotPosition(int slotNumber)
	{
		if (0 <= slotNumber && slotNumber < mRelativeSlotDisplacements.Length){
			// If not taking cover (which is urgent), return the next anchor position offset by
			// the slot displacement and rotated by the current orientation of the team. If the team
			// is taking cover, return the destination offset by the slot displacement.
			if (mCurrentFireTeamFormation != FireTeamFormation.COVER) {
				return mNextAnchorPosition +
				(mCurrentOrientation * mRelativeSlotDisplacements [slotNumber]);
			} else {
				return mDestination +
					(mCurrentOrientation * mRelativeSlotDisplacements [slotNumber]);
			}
		}
		return Vector3.zero;
	}

	private void UpdateFormation()
	{
		// Set the anchor point based on the current members of the team.
		SetAnchorPoint();
		// Assign the slot positions of the current members in the team.
		AssignSlotPositions();
	}

	private void SetAnchorPoint()
	{
		// Get the average position of all the current fire team members to set as the anchor point.
		Vector3 positionSum = Vector3.zero;
		if(mFireTeamLeader != null){
			positionSum += mFireTeamLeader.Position; // Add the fire team leader position.
		}
		for (int i = 0; i < mNonLeaderMemberCount; ++i) {
			positionSum += mFireTeamNonLeaderMembers[i].Position; // Add the fire team non-leader position.
		}
		mCurrentAnchorPosition = positionSum / (mNonLeaderMemberCount + 1); // include leader position.
	}

	private void AssignSlotPositions()
	{
		// Assign the leader to slot position zero.
		if (mFireTeamLeader != null) {
			mFireTeamLeader.slotPosition = 0;
		}
		// Assign each slot to the member in the fire team according to 
		// their position in the list of non-leader team members.
		for(int nextSlot = 1; nextSlot < mRelativeSlotDisplacements.Length; nextSlot++){
			for (int i = 0; i < mNonLeaderMemberCount; ++i) {
				// Slot positions of team members in the non-leader list are offset by 1.
				mFireTeamNonLeaderMembers [i].slotPosition = i + 1; 
			}
		}
	}

	private void SetNextAnchorPointTarget()
	{
		// Set future anchor position to be between the current anchor position and the 
		// destination by a certain amount (2x max speed of team) from 
		// the current anchor position.
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

	private bool IsFireTeamInPosition()
	{
		// Check if each fire team member is close enough to their slot position. 
		// If their are not return false.
		if (mFireTeamLeader == null) {
			// If there is no leader, assume the remainder of the team is in position.
			return true;
		}
		// Check if leader is close enough to the leader's slot position.
		if(Vector3.Distance(mFireTeamLeader.Position, 
			getSlotPosition(mFireTeamLeader.slotPosition)) > mkMinDistanceFromSlotPositionNeeded)
		{
			return false;
		}
		// Check if non-leaders are close enough to their assigned slot positions.
		for (int i = 0; i < mNonLeaderMemberCount; ++i) {
			FireTeamAlly fireTeamAlly = mFireTeamNonLeaderMembers [i];
			if(Vector3.Distance(fireTeamAlly.Position, 
				getSlotPosition(fireTeamAlly.slotPosition)) > mkMinDistanceFromSlotPositionNeeded)
			{
				return false;
			}
		}

		// If all fire team members are close enough to their slot positions, return true;
		return true;
	}

	// Returns the ally that replaces the removed ally in the team ordering.
	private FireTeamAlly RemoveFireTeamAllyFromTeamMemberList(FireTeamAlly fireTeamAllyToRemove)
	{
		// If the added ally is the leader, remove it as the leader. Otherwise, 
		// remove the ally from the list of non-leaders.
		if (fireTeamAllyToRemove == mFireTeamLeader) {
			mFireTeamLeader = null;
			// Set the new team leader to be the first team member in the
			// non-leader team  member list, if one exists.
			if (mNonLeaderMemberCount > 0) {
				mFireTeamLeader = mFireTeamNonLeaderMembers [0];
				// Remove the new team leader from the list of non-leader team members
				// and shift the other team members to fill the empty spot.
				RemoveTeamMemberAtIndex(0);
				return mFireTeamLeader;
			}
		} else {
			// Try to find the ally in the list of non-leader team members.
			int indexOfTeamMemberToRemove = -1;
			for (int i = 0; i < mNonLeaderMemberCount; i++) {
				if (fireTeamAllyToRemove == mFireTeamNonLeaderMembers [i]) {
					indexOfTeamMemberToRemove = i;
					break;
				}
			}
			// Remove the found team member and shift the other team members.
			if (indexOfTeamMemberToRemove > -1 && 
				indexOfTeamMemberToRemove < mFireTeamNonLeaderMembers.Length) {
				RemoveTeamMemberAtIndex (indexOfTeamMemberToRemove);
				// Return the new ally at the index of the removed ally.
				return 	mFireTeamNonLeaderMembers [indexOfTeamMemberToRemove];
			}
		}
		return null;
	}

	private void InsertTeamMemberAtIndex(FireTeamAlly fireTeamAllyToInsert, int index)
	{
		// If the index is already being used, shift team members to make room for 
		// the team member to add without them becoming overwritten themselves.
		if (mFireTeamNonLeaderMembers [index] != null) {
			for (int i = mNonLeaderMemberCount; i > index; --i) {
				mFireTeamNonLeaderMembers [i] = mFireTeamNonLeaderMembers [i - 1];
			}
			// Insert the fire team member into the specified index.
			fireTeamAllyToInsert.fireTeamRole = FireTeamRole.NON_LEADER;
			mFireTeamNonLeaderMembers [index] = fireTeamAllyToInsert;

		} else {
			// Fill the first empty slot found in the list of non-leader
			// team members. This ensures there are no slot gaps in 
			// the formation.
			for (int i = 0; i < mFireTeamNonLeaderMembers.Length; ++i) {
				if (mFireTeamNonLeaderMembers [i] == null) {
					mFireTeamNonLeaderMembers [i] = fireTeamAllyToInsert;
					break;
				}
			}
		}
		// Increment the non-leader team member counter
		++mNonLeaderMemberCount;
	}

	private void RemoveTeamMemberAtIndex(int index)
	{
		// Remove the team member at the specified index and shift the other team members 
		// by shifting the other team members by one position, overwriting the removed 
		// team member.
		for (int i = index; i < (mNonLeaderMemberCount - 1); ++i) {
			mFireTeamNonLeaderMembers [i] = mFireTeamNonLeaderMembers [i + 1];
		}
		// Null the value at the index of the last fire team member shifted.
		mFireTeamNonLeaderMembers [mNonLeaderMemberCount - 1] = null;
		// Decrement the non-leader team member counter
		--mNonLeaderMemberCount;
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