using System;
using System.Collections.Generic;
using UnityEngine;

public enum FireTeamFormation {WEDGE};

public class FireTeam
{
	public Vector3 destination;
	public int teamNumber;
	private FireTeamFormation mCurrentFireTeamFormation;
	private List<FireTeamAlly> mFireTeamMembers;
	private Vector3[] mRelativeSlotDisplacements;
	private Vector3 mCurrentAnchorPosition;
	private Quaternion currentOrientation;

	public FireTeamFormation CurrentFireTeamFormation
	{
		get
		{ 
			return mCurrentFireTeamFormation; 
		}
	}

	public FireTeam ()
	{
		destination = Vector3.zero;
		teamNumber = 0;
		mCurrentFireTeamFormation = FireTeamFormation.WEDGE;
		mFireTeamMembers = new List<FireTeamAlly> ();
		mRelativeSlotDisplacements = new Vector3[4];
		mCurrentAnchorPosition = Vector3.zero;
		currentOrientation = Quaternion.identity;
	}

	public void AddFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		mFireTeamMembers.Add (fireTeamAlly);
	}

	public void RemoveFireTeamAlly(FireTeamAlly fireTeamAlly)
	{
		mFireTeamMembers.Remove(fireTeamAlly);
	}

	public Vector3 getSlotPosition(int slotNumber)
	{
		if (0 <= slotNumber && slotNumber < mRelativeSlotDisplacements.Length)
		{
			return (destination + mRelativeSlotDisplacements[slotNumber]);
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
			AssignSlotPositions ();
			break;
		}
	}
		
	public void AssignSlotPositions()
	{
		// This should assign slot positions in a smarter way. For now, it just assigns the slot positions
		// based on the order of the team members in the list of team members.
		int slotPositionCount = 0;
		foreach (FireTeamAlly fireTeamAlly in mFireTeamMembers)
		{
			fireTeamAlly.slotPosition = slotPositionCount;
			++slotPositionCount;
		}
	}

	private void SetAnchorPoint()
	{
		// Get the average position of all the current fire team members to set as the anchor point.
		Vector3 positionSum = Vector3.zero;
		foreach (FireTeamAlly teamMember in mFireTeamMembers) {
			positionSum += teamMember.Position;
		}
		mCurrentAnchorPosition = positionSum / mFireTeamMembers.Count;
	}

	private void SetWedgeSlotPositions()
	{
		// Set the relative displacements of each of the slots from the anchor point.
		mRelativeSlotDisplacements[0] = new Vector3(5, 0, 0);
		mRelativeSlotDisplacements [1] = new Vector3 (0, 0, -5);
		mRelativeSlotDisplacements [2] = new Vector3 (0, 0, 5);
		mRelativeSlotDisplacements [3] = new Vector3 (-5, 0, 10);
	}
}

