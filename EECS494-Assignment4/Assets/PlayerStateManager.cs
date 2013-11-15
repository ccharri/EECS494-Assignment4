using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateManager : MonoBehaviour 
{
//	private Dictionary<string, PlayerState> playerStateMap;
//	private Dictionary<string, List<Unit>> playerUnitMap;
//	private Dictionary<string, SpawnerState> playerSpawnerStateMap;
//
//	public PlayerState getPlayerState(string playerID)
//	{
//		return playerStateMap[playerID];
//	}
//
//	public void addPlayerState(PlayerState pstate)
//	{
//		playerStateMap.Add(pstate.pid, pstate);
//	}
//
//	public List<Unit> getPlayerUnits(string playerID)
//	{
//		return playerUnitMap[playerID];
//	}
//
//	public void addUnit(Unit u)
//	{
//		List<Unit> list;
//		string id = u.getOwnerID();
//		if((list = playerUnitMap[id]) == null)
//			playerUnitMap.Add(id, new List<Unit>());
//
//		playerUnitMap[id].Add(u);
//	}
//
//	public SpawnerState getSpawnerState(string playerID)
//	{
//		return playerSpawnerStateMap[playerID];
//	}
//
//	public void addSpawnerState(SpawnerState spawn)
//	{
//		playerSpawnerStateMap.Add(spawn.getOwnerID(), spawn);
//	}
//
//	public void initializePlayer(string playerID)
//	{
//		playerStateMap.Add (playerID, new PlayerState(playerID));
//		playerUnitMap.Add(playerID, new List<Unit>());
//		playerSpawnerStateMap.Add(playerID, new SpawnerState(playerID));
//
//
//	}
//
//	private void removePlayer(string playerID)
//	{
//		playerStateMap.Remove(playerID);
//		playerUnitMap.Remove(playerID);
//		playerSpawnerStateMap.Remove(playerID);
//	}
//	
//	void OnPlayerConnected(NetworkPlayer player)
//	{
//		initializePlayer(player.guid);
//	}
//	
//	void OnPlayerDisconnected(NetworkPlayer player)
//	{
//		
//	}
//
//
//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//

}
