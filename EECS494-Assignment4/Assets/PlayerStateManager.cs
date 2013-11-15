﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStateManager : MonoBehaviour 
{
	private Dictionary<string, PlayerState> playerStateMap;
	private Dictionary<string, List<Unit>> playerUnitMap;
	private Dictionary<string, SpawnerState> playerSpawnerStateMap;

	public PlayerState getPlayerState(string playerID)
	{
		return playerStateMap[playerID];
	}

	public void addPlayerState(PlayerState pstate)
	{
		playerStateMap.Add(pstate.pid, pstate);
	}

	public List<Unit> getPlayerUnits(string playerID)
	{
		return playerUnitMap[playerID];
	}

	public void addUnit(Unit u)
	{
		List<Unit> list;
		string id = u.getOwnerID();
		if((list = playerUnitMap[id]) == null)
			playerUnitMap.Add(id, new List<Unit>());

		playerUnitMap[id].Add(u);
	}

	public SpawnerState getSpawnerState(string playerID)
	{
		return playerSpawnerStateMap[playerID];
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

	private void removePlayer(string playerID)
	{
		playerStateMap.Remove(playerID);
		playerUnitMap.Remove(playerID);
		playerSpawnerStateMap.Remove(playerID);
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

	//RPCs

	//Client RPCs
	
	//setGold
	[RPC]
	void setGold(int gold_, string guid_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive these setGold RPC calls!");
			return;
		}

		if(Network.isClient && (guid_ != Network.player.guid))
		{
			Debug.Log ("Received setGold for wrong player!");
			return;
		}

		Debug.Log ("setGold received from " + info.sender + ", amount = " + gold_ + ", player = " + guid_);	
		setGold(gold_, guid_);
	}
	
	void setGold(int gold_, string guid_)
	{
		PlayerState state = playerStateMap[guid_];
		if(state == null) {Debug.Log("Player " + guid_ + " not found!"); return;}
		state.gold = gold_;
	}
	
	//setIncome
	[RPC]
	void setIncome(int income_, string guid_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive these setIncome RPC calls!");
			return;
		}

		if(Network.isClient && (guid_ != Network.player.guid))
		{
			Debug.Log ("Received setIncome for wrong player!");
			return;
		}

		Debug.Log ("setIncome received from " + info.sender + ", income = " + income_ + ", player = " + guid_);
		setIncome (income_, guid_);
	}
	
	void setIncome(int income_, string guid_)
	{
		PlayerState state = playerStateMap[guid_];
		if(state == null) {Debug.Log("Player " + guid_ + " not found!"); return;}
		state.income = income_;
	}

	//setLives
	[RPC]
	void setLives(int lives_, string guid_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive these setLives RPC calls!");
			return;
		}

		Debug.Log ("setLives received from " + info.sender + ", lives = " + lives_ + ", player = " + guid_);
		setLives(lives_, guid_);
	}
	
	void setLives(int lives_, string guid_)
	{
		PlayerState state = playerStateMap[guid_];
		if(state == null) {Debug.Log("Player " + guid_ + " not found!"); return;}
		state.lives = lives_;
	}
}
