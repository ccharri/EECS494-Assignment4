using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
	Dictionary<string, List<Creep>> creepsByArena;
    //NOTE: Returns the creeps in a player's arena. Indexed by pid

	Dictionary<string, List<Tower>> towersByPlayer;
    //NOTE: Returns towers owned by a player. Indexed by pid.

	Dictionary<string, PlayerState> players;

	Dictionary<string, SpawnerState> spawns;

    float incomeTimeIncrement = 10;
    float nextIncomeTime = 10;
    float time = 0;

    void Start()
    {
        creepsByArena = new Dictionary<string, List<Creep>>();
        towersByPlayer = new Dictionary<string, List<Tower>>();
        players = new Dictionary<string, PlayerState>();
        spawns = new Dictionary<string, SpawnerState>();
    }

    void FixedUpdate()
    {
		if(Network.isServer)
		{
	        time += Time.deltaTime;
	        if(time >= nextIncomeTime)
	        {
	            foreach(var p in players)
				{
					PlayerState ps = p.Value;
	               	ps.gold += ps.income;

					//We know we're the server, so if the player we're updating isn't us, go ahead and RPC
					if(ps.player != Network.player)
						networkView.RPC ("setGold", ps.player, ps.gold, ps.player.guid);
				}
	            nextIncomeTime += incomeTimeIncrement;

				//Update everyone else's timer.  We don't want to have to rely on messages passed back in,
				//	in case a User can fabricate setIncomeTimer messages from the server itself
				networkView.RPC("setIncomeTimer", RPCMode.OthersBuffered, nextIncomeTime);
	        }
		}
    }

	public void addCreepForPlayer(string pid, Creep c) //TODO: fix params? make it a prefab? change the method name?
    {
		List<Creep> list = creepsByArena[pid];
		if(list != null)
			list.Add(c);
    }
	public void addTowerForPlayer(string pid, Tower t) //TODO: fix params? make it a prefab? change the method name?
    {
		List<Tower> list = towersByPlayer[pid];
        if(list != null)
            list.Add(t);
    }

	public void removeCreepForPlayer(string pid, Creep c)
	{
		List<Creep> list = creepsByArena[pid];
		if(list != null)
			list.Remove(c);
	}

	public void removeTowerForPlayer(string pid, Tower t)
	{
		List<Tower> list = towersByPlayer[pid];
		if(list != null)
			list.Remove(t);
	}
	
	public List<Creep> getEnemyCreeps(string pid)
    {
     //   if(creepsByArena.ContainsKey(pid))
            return creepsByArena[pid];
      //  return null; //TODO: Throw an exception?
    }

	public void addPlayer(NetworkPlayer player)
    {
        creepsByArena.Add(player.guid, new List<Creep>());
        towersByPlayer.Add(player.guid, new List<Tower>());
        players.Add(player.guid, new PlayerState(player));
		spawns.Add (player.guid, new SpawnerState(player.guid));
    }

    

	void OnPlayerConnected(NetworkPlayer player)
	{
		if(Network.isServer)
		{
			initializePlayer(player);
			networkView.RPC("initializePlayer", RPCMode.OthersBuffered, player);
		}
	}
		
	void OnPlayerDisconnected(NetworkPlayer player)
	{
//		if(Network.isServer)
//		{
//			removePlayer(player.guid);
//			networkView.RPC ("removePlayer", RPCMode.OthersBuffered, player.guid);
//		}
	}

    public float getGameTime() { return time; }



	//RPCs

	//-----------------------------------------------------
	//Client RPCs
	//-----------------------------------------------------

	//-----------------------------------------------------
	//Player adding
	//-----------------------------------------------------

	[RPC]
	void initializePlayer(NetworkPlayer player_, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive initializePlayer RPC calls!");
			return;
		}

		Debug.Log ("initialzePlayer received from " + info_.sender + ", player = " + player_);	
		initializePlayer(player_, info_);
	}

	void initializePlayer(NetworkPlayer player)
	{
		players.Add (player.guid, new PlayerState(player));
		creepsByArena.Add(player.guid, new List<Creep>());
		towersByPlayer.Add(player.guid, new List<Tower>());
		spawns.Add(player.guid, new SpawnerState(player.guid));
	}

	//Player removing

	[RPC]
	void removePlayer(string playerID, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive removePlayer RPC calls!");
			return;
		}

		Debug.Log ("removePlayer received from " + info_.sender + ", player = " + playerID);
		removePlayer (playerID);
	}

	void removePlayer(string playerID)
	{
		players.Remove(playerID);
		creepsByArena.Remove(playerID);
		towersByPlayer.Remove(playerID);
		spawns.Remove(playerID);
	}

	//-----------------------------------------------------
	//Tower and Creep List Modifiers
	//-----------------------------------------------------

	//Add Tower

	[RPC]
	void addTower(NetworkViewID networkViewID, string ownerGUID_, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive addTower RPC calls!");
			return;
		}

		Debug.Log ("addTower received from " + info_.sender + ", owner = " + ownerGUID_ + ", NetworkViewID = " + networkViewID);
		addTower (networkViewID, ownerGUID_);
	}

	void addTower(NetworkViewID networkViewID, string ownerGUID_)
	{
		NetworkView view = NetworkView.Find(networkViewID);
		addTowerForPlayer(ownerGUID_, view.gameObject.GetComponent<Tower>());
	}

	//Add Creep

	[RPC]
	void addCreep(NetworkViewID networkViewID, string ownerGUID_, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive addCreep RPC calls!");
			return;
		}
		
		Debug.Log ("addCreep received from " + info_.sender + ", owner = " + ownerGUID_ + ", NetworkViewID = " + networkViewID);
		addCreep (networkViewID, ownerGUID_);
	}
	
	void addCreep(NetworkViewID networkViewID, string ownerGUID_)
	{
		NetworkView view = NetworkView.Find(networkViewID);
		addCreepForPlayer(ownerGUID_, view.gameObject.GetComponent<Creep>());
	}

	//Remove Tower
	[RPC]
	void removeTower(NetworkViewID networkViewID, string ownerGUID_, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive removeTower RPC calls!");
			return;
		}

		Debug.Log ("removeTower recieved from " + info_.sender + ", owner = " + ownerGUID_ + ", NetworkViewID = " + networkViewID);
		removeTower (networkViewID, ownerGUID_);
	}

	void removeTower(NetworkViewID networkViewID, string ownerGUID_)
	{
		NetworkView view = NetworkView.Find (networkViewID);
		removeTowerForPlayer(ownerGUID_, view.gameObject.GetComponent<Tower>());
	}

	//Remove Creep
	[RPC]
	void removeCreep(NetworkViewID networkViewID, string ownerGUID_, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive removeCreep RPC calls!");
			return;
		}
		
		Debug.Log ("removeCreep recieved from " + info_.sender + ", owner = " + ownerGUID_ + ", NetworkViewID = " + networkViewID);
		removeCreep (networkViewID, ownerGUID_);
	}
	
	void removeCreep(NetworkViewID networkViewID, string ownerGUID_)
	{
		NetworkView view = NetworkView.Find (networkViewID);
		removeCreepForPlayer(ownerGUID_, view.gameObject.GetComponent<Creep>());
	}


	//-----------------------------------------------------
	//PlayerState setters
	//-----------------------------------------------------

	//setIncomeTimer
	[RPC]
	void setIncomeTimer(float time_, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive setIncomeTimer RPC calls!");
			return;
		}
		nextIncomeTime = time_;
	}
	
	//setGold
	[RPC]
	void setGold(int gold_, string guid_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive setGold RPC calls!");
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
		PlayerState state = players[guid_];
		if(state == null) {Debug.Log("Player " + guid_ + " not found!"); return;}
		state.gold = gold_;
	}
	
	//setIncome
	[RPC]
	void setIncome(int income_, string guid_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive setIncome RPC calls!");
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
		PlayerState state = players[guid_];
		if(state == null) {Debug.Log("Player " + guid_ + " not found!"); return;}
		state.income = income_;
	}
	
	//setLives
	[RPC]
	void setLives(int lives_, string guid_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive setLives RPC calls!");
			return;
		}
		
		Debug.Log ("setLives received from " + info.sender + ", lives = " + lives_ + ", player = " + guid_);
		setLives(lives_, guid_);
	}
	
	void setLives(int lives_, string guid_)
	{
		PlayerState state = players[guid_];
		if(state == null) {Debug.Log("Player " + guid_ + " not found!"); return;}
		state.lives = lives_;
	}
}
