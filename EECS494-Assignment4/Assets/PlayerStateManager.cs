using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateManager : MonoBehaviour {
	private Hashtable playerStateMap;
	private Hashtable playerUnitMap;
	private Hashtable playerSpawnerStateMap;

	public PlayerState getPlayerState(string playerID)
	{
		return (PlayerState)playerStateMap[playerID];
	}

	public void addPlayerState(PlayerState pstate)
	{
		playerStateMap.Add(pstate.pid, pstate);
	}

	public List<Unit> getPlayerUnits(string playerID)
	{
		return (List<Unit>)playerUnitMap[playerID];
	}

	public void addUnit(Unit u)
	{
		List<Unit> list;
		string id = u.getOwnerID();
		if((list = (List<Unit>)playerUnitMap[id]) == null)
			playerUnitMap.Add (id, new List<Unit>());

		((List<Unit>)playerUnitMap[id]).Add(u);
	}

	public SpawnerState getSpawnerState(string playerID)
	{
		return (SpawnerState)playerSpawnerStateMap[playerID];
	}

	public void addSpawnerState(SpawnerState spawn)
	{
		playerSpawnerStateMap.Add(spawn.getOwnerID(), spawn);
	}

	public void initializePlayer(string playerID)
	{
		playerStateMap.Add (playerID, new PlayerState(playerID));
		playerUnitMap.Add(playerID, new List<Unit>());
		playerSpawnerStateMap.Add(playerID, new SpawnerState(playerID));
	}

	
	void OnPlayerConnected(NetworkPlayer player)
	{
		initializePlayer(player.guid);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
