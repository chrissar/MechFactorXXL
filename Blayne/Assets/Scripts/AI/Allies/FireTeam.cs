using System;
using System.Collections.Generic;
using UnityEngine;

public enum FireTeamFormation {WEDGE, FILE};

public class FireTeam
{
	private const float kMinDistanceFromSlotPositionNeeded = 5.0f;

	public int teamNumber;

	private FireTeamFormation mCurrentFireTeamFormation;
	private Vector3 mDestination;
	private FireTeamAlly mFireTeamLeader;
	private int mNonTeamMemberCount;
	private FireTeamAlly[] mFireTeamNonLeaderMembers;
	private Vector3[] mRelativeSlotDisplacements;
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

	public FireTeam ()
	{
		teamNumber = 0;
		mDestination = Vector3.zero;
		mCurrentFireTeamFormation = FireTeamFormation.WEDGE;
		mFireTeamLeader = null;
		mNonTeamMemberCount = 0;
		mFireTeamNonLeaderMembers = new FireTeamAlly[3];
		mRelativeSlotDisplacements = new Vector3[4];
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

	public void AddFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		if (fireTeamAlly == null) {
			return;
		}
		// If the added ally is a leader, set it as the leader if there is currently no leader.
		// Otherwise, add it to the list of non-leaders.
		if (mFireTeamLeader == null && fireTeamAlly.fireTeamRole == FireTeamRole.LEADER) {
			mFireTeamLeader = fireTeamAlly;
		} else if(mNonTeamMemberCount < mFireTeamNonLeaderMembers.Length){
			// Add team member to list of non-leader team members if there is room.
			mFireTeamNonLeaderMembers[mNonTeamMemberCount] = fireTeamAlly;
			mNonTeamMemberCount++;
		}
		// Update the formation to account for the added member.
		UpdateFormation();
	}

	// Returns the ally that replaces the removed ally, including a replaced leader, but 
	// can return null if there were no replacement allies.
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
		
	public Vector3 getSlotPosition(int slotNumber)
	{
		if (0 <= slotNumber && slotNumber < mRelativeSlotDisplacements.Length){
			// Return the next anchor position offset by the slot displacement and
			// rotated by the current orientation of the team.
			return mNextAnchorPosition +
				(mCurrentOrientation * mRelativeSlotDisplacements[slotNumber]);
		}
		return Vector3.zero;
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
				break;
			case FireTeamFormation.FILE:
				// Set the slot positions for the file formation.
				SetFileSlotPositions ();
				break;
		}
		// Assign the slot positions to the members of the fire team.
		AssignSlotPositions ();
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
		for (int i = 0; i < mNonTeamMemberCount; ++i) {
			positionSum += mFireTeamNonLeaderMembers[i].Position; // Add the fire team non-leader position.
		}
		mCurrentAnchorPosition = positionSum / (mNonTeamMemberCount + 1); // include leader position.
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
			for (int i = 0; i < mNonTeamMemberCount; ++i) {
				// Slot positions of team members in the non-leader list are offset by 1.
				mFireTeamNonLeaderMembers [i].slotPosition = i + 1; 
			}
		}
	}
		
	private void SetWedgeSlotPositions()
	{
		// Set the relative displacements of each of the slots from the anchor point.
		mRelativeSlotDisplacements[0] = new Vector3(5, 0, 0);
		mRelativeSlotDisplacements [1] = new Vector3 (0, 0, -5);
		mRelativeSlotDisplacements [2] = new Vector3 (0, 0, 5);
		mRelativeSlotDisplacements [3] = new Vector3 (-5, 0, 10);
	}

	private void SetFileSlotPositions()
	{
		// Set the relative displacements of each of the slots from the anchor point.
		mRelativeSlotDisplacements[0] = new Vector3(0, 0, 0);
		mRelativeSlotDisplacements [1] = new Vector3 (5, 0, 0);
		mRelativeSlotDisplacements [2] = new Vector3 (-5, 0, 0);
		mRelativeSlotDisplacements [3] = new Vector3 (-10, 0, 0);
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
			getSlotPosition(mFireTeamLeader.slotPosition)) > kMinDistanceFromSlotPositionNeeded)
		{
			return false;
		}
		// Check if non-leaders are close enough to their assigned slot positions.
		for (int i = 0; i < mNonTeamMemberCount; ++i) {
			FireTeamAlly fireTeamAlly = mFireTeamNonLeaderMembers [i];
			if(Vector3.Distance(fireTeamAlly.Position, 
				getSlotPosition(fireTeamAlly.slotPosition)) > kMinDistanceFromSlotPositionNeeded)
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
			if (mNonTeamMemberCount > 0) {
				mFireTeamLeader = mFireTeamNonLeaderMembers [0];
				// Remove the new team leader from the list of non-leader team members
				// and shift the other team members to fill the empty spot.
				RemoveTeamMemberAtIndex(0);
				mFireTeamLeader.fireTeamRole = FireTeamRole.LEADER; // Set as having leader role.
				return mFireTeamLeader;
			}
		} else {
			// Try to find the ally in the list of non-leader team members.
			int indexOfTeamMemberToRemove = -1;
			for (int i = 0; i < mNonTeamMemberCount; i++) {
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

	private void RemoveTeamMemberAtIndex(int index)
	{
		// Remove the team member at the specified index and shift the other team members 
		// by shifting the other team members by one position, overwriting the removed 
		// team member.
		for (int i = index; i < (mNonTeamMemberCount - 1); ++i) {
			mFireTeamNonLeaderMembers [i] = mFireTeamNonLeaderMembers [i + 1];
		}
		// Null the value at the index of the last fire team member shifted.
		mFireTeamNonLeaderMembers [mNonTeamMemberCount - 1] = null;
		// Decrement the team member counter
		--mNonTeamMemberCount;
	}
}

