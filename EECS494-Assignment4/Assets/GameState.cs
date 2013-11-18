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

	void Awake()
	{
		//Should be set higher than default if going to use more than 100 networkViewID requests/minute
		//As this is based on spawning lots of units, I have a feeling we will need as many viewIDs as we can get
		//in the viewID pool.  Thus, the reason this is at 300;
		Network.minimumAllocatableViewIDs = 300;
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
	                p.Value.gold += p.Value.income;
					networkView.RPC ("setGold", p.Value.player, p.Value.gold, p.Value.player.guid);
				}
	            nextIncomeTime += incomeTimeIncrement;
				networkView.RPC("setIncomeTimer", RPCMode.AllBuffered, nextIncomeTime);
	        }
		}
    }

	public void spawnCreepForPlayer(string pid, Creep c) //TODO: fix params? make it a prefab? change the method name?
    {
        if(creepsByArena.ContainsKey(pid))
            creepsByArena[pid].Add(c);
    }
	public void spawnTowerForPlayer(string pid, Tower t) //TODO: fix params? make it a prefab? change the method name?
    {
        if(towersByPlayer.ContainsKey(pid))
            towersByPlayer[pid].Add(t);
    }
	
	public List<Creep> getEnemyCreeps(string pid)
    {
        if(creepsByArena.ContainsKey(pid))
            return creepsByArena[pid];
        return null; //TODO: Throw an exception?
    }

	public void addPlayer(NetworkPlayer player)
    {
        creepsByArena.Add(player.guid, new List<Creep>());
        towersByPlayer.Add(player.guid, new List<Tower>());
        players.Add(player.guid, new PlayerState(player));
		spawns.Add (player.guid, new SpawnerState(player.guid));
    }

    public float getGameTime() { return time; }

    private static GameState instance;
    private GameState() 
    {
		creepsByArena = new Dictionary<string, List<Creep>>();
		towersByPlayer = new Dictionary<string, List<Tower>>();
		players = new Dictionary<string, PlayerState>();
		spawns = new Dictionary<string, SpawnerState>();
    }

	void OnPlayerConnected(NetworkPlayer player)
	{
		initializePlayer(player);
	}
		
	void OnPlayerDisconnected(NetworkPlayer player)
	{
//		removePlayer(player.guid);
	}

	public void initializePlayer(NetworkPlayer player)
	{
		players.Add (player.guid, new PlayerState(player));
		creepsByArena.Add(player.guid, new List<Creep>());
		towersByPlayer.Add(player.guid, new List<Tower>());
		spawns.Add(player.guid, new SpawnerState(player.guid));
	}

	private void removePlayer(string playerID)
	{
		players.Remove(playerID);
		creepsByArena.Remove(playerID);
		towersByPlayer.Remove(playerID);
		spawns.Remove(playerID);
	}

	//RPCs
	
	//Client RPCs

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
