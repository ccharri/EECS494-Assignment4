using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerState
{
	protected NetworkPlayer owner;
	private Dictionary<string, UnitSpawn> unitSpawnStateMap;

	public NetworkPlayer getOwner() {return owner;}

	public SpawnerState(NetworkPlayer owner_)
	{
		owner = owner_;
		unitSpawnStateMap = new Dictionary<string, UnitSpawn>();
	}

	//Disable default construction
	private SpawnerState() {}

	public UnitSpawn getSpawn(Spawnable u)
	{
		return unitSpawnStateMap[u.name];
	}

	public UnitSpawn getSpawn(string name)
	{
		return unitSpawnStateMap[name];
	}

	public void addSpawn(UnitSpawn spawn)
	{
		unitSpawnStateMap.Add(spawn.spawn.name, spawn);
	}

	public Dictionary<string, UnitSpawn>.ValueCollection getSpawns()
	{
		return unitSpawnStateMap.Values;
	}

	public Dictionary<string, UnitSpawn>.KeyCollection getKeys()
	{
		return unitSpawnStateMap.Keys;
	}
}
