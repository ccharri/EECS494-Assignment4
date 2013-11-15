using UnityEngine;
using System.Collections;

public class SpawnerState
{
	protected string ownerID;
	private Hashtable unitSpawnStateMap;

	public string getOwnerID() {return ownerID;}

	public SpawnerState(string ownerID_)
	{
		ownerID = ownerID_;
	}

	//Disable default construction
	private SpawnerState() {}

	public UnitSpawn getSpawn(Spawnable u)
	{
		return (UnitSpawn)unitSpawnStateMap[u.name];
	}

	public void addSpawn(UnitSpawn spawn)
	{
		unitSpawnStateMap.Add(spawn.spawn.name, spawn);
	}
}
