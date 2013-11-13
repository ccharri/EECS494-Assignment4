using UnityEngine;
using System.Collections;

public class SpawnerState
{
	protected int ownerID;
	private Hashtable unitSpawnStateMap;

	public int getOwnerID() {return ownerID;}

	public SpawnerState(int ownerID_)
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
