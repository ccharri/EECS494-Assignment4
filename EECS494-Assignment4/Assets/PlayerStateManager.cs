using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateManager : MonoBehaviour {
	private Hashtable playerStateMap;
	private Hashtable playerUnitMap;
	private Hashtable playerSpawnerStateMap;

	public PlayerState getPlayerState(int playerID)
	{
		return (PlayerState)playerStateMap[playerID];
	}

	public void addPlayerState(PlayerState pstate)
	{
		playerStateMap.Add(pstate.pid, pstate);
	}

	public List<Unit> getPlayerUnits(int playerID)
	{
		return (List<Unit>)playerUnitMap[playerID];
	}

	public void addUnit(Unit u)
	{
		List<Unit> list;
		int id = u.getOwnerID();
		if((list = (List<Unit>)playerUnitMap[id]) == null)
			playerUnitMap.Add (id, new List<Unit>());

		((List<Unit>)playerUnitMap[id]).Add(u);
	}

	public SpawnerState getSpawnerState(int playerID)
	{
		return (SpawnerState)playerSpawnerStateMap[playerID];
	}

	public void addSpawnerState(SpawnerState spawn)
	{
		playerSpawnerStateMap.Add(spawn.getOwnerID(), spawn);
	}

	public void initializePlayer(int playerID)
	{
		/*
		playerStateMap.Add (playerID, new PlayerState(playerID));
		playerUnitMap.Add(playerID, new List<Unit>());
		playerSpawnerStateMap.Add(playerID, new SpawnerState(playerID));
		*/
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
