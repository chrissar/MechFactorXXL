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
	private List<FireTeamAlly> mFireTeamNonLeaderMembers;
	private Vector3[] mRelativeSlotDisplacements;
	private Vector3 mCurrentAnchorPosition;
	private Vector3 mNextAnchorPosition; // Slightly ahead of anchor point to set target slot positions.
	private Quaternion mCurrentOrientation;
	private float currentSpeed;

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

	public void setDestination(Vector3 destination)
	{
		mDestination = destination;
		setNextAnchorPointTarget ();
		setOrientation ();
	}

	public FireTeam ()
	{
		mDestination = Vector3.zero;
		teamNumber = 0;
		mCurrentFireTeamFormation = FireTeamFormation.WEDGE;
		mFireTeamLeader = null;
		mFireTeamNonLeaderMembers = new List<FireTeamAlly> ();
		mRelativeSlotDisplacements = new Vector3[4];
		mCurrentAnchorPosition = Vector3.zero;
		mNextAnchorPosition = mCurrentAnchorPosition;
		mCurrentOrientation = Quaternion.identity;
		currentSpeed = 2.0f;
	}

	public void UpdateAnchor()
	{
		// Only move the anchor if the fire team is close enough to their slot positions.
		if (isFireTeamInPosition ()) {
			// Update current anchor point to move towards destination based on overall team speed.
			Vector3 destinationDisplacement = mDestination - mCurrentAnchorPosition;
			mCurrentAnchorPosition += destinationDisplacement.normalized * currentSpeed * Time.deltaTime;
			setNextAnchorPointTarget ();
		}
	}

	public void AddFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		if (fireTeamAlly == null) {
			return;
		}
		// If the added ally is a leader, set it as the leader. Otherwise, add it to the list of non-leaders.
		if (fireTeamAlly.fireTeamRole == FireTeamRole.LEADER) {
			mFireTeamLeader = fireTeamAlly;
		} else {
			mFireTeamNonLeaderMembers.Add (fireTeamAlly);
		}
	}

	public void RemoveFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		if (fireTeamAlly == null) {
			return;
		}
		// If the added ally is the leader, remove it as the leader. Otherwise, 
		// remove the ally from the list of non-leaders.
		if (fireTeamAlly == mFireTeamLeader) {
			mFireTeamLeader = null;
		} else {
			mFireTeamNonLeaderMembers.Remove (fireTeamAlly);
		}
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
		
	public void AssignSlotPositions()
	{
		// Assign the leader to slot position zero.
		mFireTeamLeader.slotPosition = 0;
		// Reset the slot positions of each non-leader team member to know which ones have not been 
		// assigned to a slot yet.
		foreach (FireTeamAlly fireTeamAlly in mFireTeamNonLeaderMembers){
			fireTeamAlly.slotPosition = -1;
		}
		// Assign each slot to the member in the fire team that is closest to that slot. 
		for(int nextSlot = 1; nextSlot < mRelativeSlotDisplacements.Length; nextSlot++){
			FireTeamAlly closestAlly = null;
			float closestDistance = -1.0f;
			foreach (FireTeamAlly fireTeamAlly in mFireTeamNonLeaderMembers) {
				// Only consider allies that have not yet been assigned to a slot.
				if (fireTeamAlly.slotPosition == -1) {
					// Get distance from the ally to the slot to fill.
					float distance = Vector3.Distance (fireTeamAlly.Position, getSlotPosition (nextSlot));
					if (closestDistance < 0 || distance < closestDistance) {
						// Set the ally as the closest ally so far.
						closestAlly = fireTeamAlly;
						closestDistance = distance;
					}
				}
			}
			// If a closest ally was found, set its slot position to the
			// slot position currently being considered.
			if (closestAlly != null) {
				closestAlly.slotPosition = nextSlot;
			}
		}
	}

	private void SetAnchorPoint()
	{
		// Get the average position of all the current fire team members to set as the anchor point.
		Vector3 positionSum = Vector3.zero;
		positionSum += mFireTeamLeader.Position; // Add the fire team leader position.
		foreach (FireTeamAlly teamMember in mFireTeamNonLeaderMembers) {
			positionSum += teamMember.Position; // Add the fire team non-leader position.
		}
		mCurrentAnchorPosition = positionSum / mFireTeamNonLeaderMembers.Count;
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

	private void setNextAnchorPointTarget()
	{
		// Set future anchor position to be between the current anchor position and the 
		// destination by a certain amount (2x max speed of team) from 
		// the current anchor position.
		Vector3 destinationDisplacement = mDestination - mCurrentAnchorPosition;
		float destinationDistance = destinationDisplacement.magnitude;
		if (destinationDistance < 2 * currentSpeed) {
			mNextAnchorPosition = mDestination;
		} else {
			// Set the next anchor position to the point at the maximum next anchor distance from
			// the current anchor position, along the direction to the destination.
			mNextAnchorPosition = 
				mCurrentAnchorPosition + destinationDisplacement.normalized * 2 * currentSpeed;
		}
	}

	private void setOrientation()
	{
		// Set orientation to the direction that faces the destination.
		Vector3 destinationDisplacement = mDestination - mCurrentAnchorPosition;
		mCurrentOrientation = Quaternion.identity;
		mCurrentOrientation.SetFromToRotation (Vector3.right, destinationDisplacement);
	}

	private bool isFireTeamInPosition()
	{
		// Check if each fire team member is close enough to their slot position. 
		// If their are not return false.

		// Check if leader is close enough to the leader's slot position.
		if(Vector3.Distance(mFireTeamLeader.Position, 
			getSlotPosition(mFireTeamLeader.slotPosition)) > kMinDistanceFromSlotPositionNeeded)
		{
			return false;
		}
		// Check if non-leaders are close enough to their assigned slot positions.
		foreach (FireTeamAlly fireTeamAlly in mFireTeamNonLeaderMembers) {
			if(Vector3.Distance(fireTeamAlly.Position, 
				getSlotPosition(fireTeamAlly.slotPosition)) > kMinDistanceFromSlotPositionNeeded)
			{
				return false;
			}
		}

		// If all fire team members are close enough to their slot positions, return true;
		return true;
	}
}

