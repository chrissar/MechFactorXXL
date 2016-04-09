using System;
using UnityEngine;

public class Ally : MonoBehaviour, ICommandable
{
	public void executeCommand(Command command)
	{
		command.execute (this);
	}
}

