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

	List<string> playerNums;

    float incomeTimeIncrement = 10;
    float nextIncomeTime = 10;
    float time = 0;

	bool showMenu = false;

	public Vector3 spawnLocation;

	public PlacementManager pMan;
	public Camera mainCamera;
	
	void Awake()
	{
		//Should be set higher than default if going to use more than 100 networkViewID requests/minute
		//As this is based on spawning lots of units, I have a feeling we will need as many viewIDs as we can get
		//in the viewID pool.  Thus, the reason this is at 300;
		Network.minimumAllocatableViewIDs = 300;
		creepsByArena = new Dictionary<string, List<Creep>>();
		towersByPlayer = new Dictionary<string, List<Tower>>();
		players = new Dictionary<string, PlayerState>();
		spawns = new Dictionary<string, SpawnerState>();
		pMan = GetComponent<PlacementManager>();
		spawnLocation = GameObject.FindGameObjectWithTag("SpawnLocation").transform.position;
		mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		playerNums = new List<string>();
	}

  void OnLevelWasLoaded(int level)
  {
      Awake();  //maybe just copy code from awake
  }

    void Start()
    {
		initializePlayer(Network.player);
		foreach(NetworkPlayer player in Network.connections)
		{
			initializePlayer(player);
		}

		if(getPlayerNum(Network.player.guid) != 1)
		{
			Transform newPos = mainCamera.transform;
			newPos.position =  new Vector3(-newPos.position.x, newPos.position.y, newPos.position.z);
			newPos.Rotate(-270,-180,0);
		}
    }

    void FixedUpdate()
    {
		time += Time.fixedDeltaTime;

		if(Network.isServer)
		{
			//Update PlayerStates	
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

			//Update SpawnerStates
			foreach(var s in spawns)
			{
				SpawnerState ss = s.Value;
				NetworkPlayer p = ss.getOwner();

				foreach(var k in ss.getKeys())
				{
					//Need to know key, for RPC calls
					var u = ss.getSpawn(k);

					if(u.currentStock == u.maxStock) continue;

					if(time < u.initialStockTime) continue;

					if(time >= (u.lastRestock + u.restockTime))
					{
						u.currentStock += 1;
						u.lastRestock = time;

						//If it's not us
						if(p != Network.player)
						{
							networkView.RPC ("setStock", p, u.currentStock, p.guid, k);
							networkView.RPC ("setStockTimer", p, u.lastRestock, p.guid, k);
						}
					}
				}
			}

		}
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 5, Screen.width - 15, Screen.height - 5));
        GUILayout.BeginVertical();

        OnGUI_TopBar();
        OnGUI_ScoreBoard();
        GUILayout.FlexibleSpace();
        OnGUI_BottomBar();

        GUILayout.EndVertical();
        GUILayout.EndArea();

		if(showMenu)
		{
			GUILayout.Window(0, new Rect(Screen.width/2 - 200, Screen.height/2 - 200, 400, 400), WindowGUI, "Menu", GUILayout.Width(400), GUILayout.Height(400));
		}
    }

	void WindowGUI(int windowID)
	{
		GUILayout.BeginVertical();
		if(GUILayout.Button ("Back"))
		{
			showMenu = false;
		}

		GUILayout.FlexibleSpace();

		if(GUILayout.Button ("Main Menu"))
		{
			Network.Disconnect ();
			Application.LoadLevel("MainScene");
		}

		if(GUILayout.Button ("Quit Game"))
		{
			Application.Quit();
		}
		GUILayout.EndVertical();
	}

    void OnGUI_TopBar()
    {
        GUILayout.BeginHorizontal("box", GUILayout.Height(20));

        var layoutOptions = new GUILayoutOption[] { GUILayout.Height(20) };

        if (GUILayout.Button("Menu", layoutOptions))
        {
			showMenu = true;
        }
        if (GUILayout.Button("EDIT", layoutOptions))
        {

        }

        GUILayout.FlexibleSpace();

        var incomeTimerString = "Income Timer: " + ((int)(nextIncomeTime + .5) - (int)(time + .5));
        GUILayout.Label(incomeTimerString, layoutOptions);

        GUILayout.FlexibleSpace();


		int currGold = 0; 
		int currIncome = 0;
        foreach (var p in players)
        {
            if (p.Value.player.Equals(Network.player))
            {
                currGold = p.Value.gold;
                currIncome = p.Value.income;
            }
        }
        var goldIncomeString = "Gold + Income: " + currGold + " + " + currIncome;
        GUILayout.Label(goldIncomeString, layoutOptions);

        GUILayout.EndHorizontal();
    }

    void OnGUI_ScoreBoard()
    {
		int friendlyLives = 0;
		int opponentLives = 0;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        foreach (var p in players)
        {
            if (p.Value.player.Equals(Network.player))
                friendlyLives = p.Value.lives;
            else
                opponentLives = p.Value.lives;
        }

        var scoreBoardString = "Your Lives: " + friendlyLives + "\nOpponent's Lives: " + opponentLives;
        GUILayout.Box(scoreBoardString);

        GUILayout.EndHorizontal();
    }

    void OnGUI_BottomBar()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3), GUILayout.Height(Screen.height / 5));
        GUILayout.Label("Buffs: ");
        GUILayout.EndVertical();

        //GUILayout.FlexibleSpace();

        //Used to set player's towers and creeps
        PlayerState pState = players[Network.player.guid];
        int rowCount = 0;

        //TOWER PLACEMENT BUTTONS
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3), GUILayout.Height(Screen.height / 5));
	
		GUILayout.BeginHorizontal();
		foreach (KeyValuePair<string, Tower> entry in pState.race.towerMap)
		{
			rowCount++;
			if (GUILayout.Button(entry.Value.name)) //use entry.Value.name after towers have a name defined (maybe)
			{
				foreach(var e in pState.race.towerMap)
				{
					Debug.Log ("Key = " + e.Key + ", Value = " + e.Value);
					Debug.Log (e.Value.name + ", " + e.Value.id);
				}
				pMan.enabled = true;
				pMan.beginPlacing(entry.Value.prefab);
			}
			if (rowCount == 4)
			{
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				rowCount = 0;
			}
		}
		GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        //GUILayout.FlexibleSpace();

        //CREEP PLACEMENT BUTTONS
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3 - 30), GUILayout.Height(Screen.height / 5));

		rowCount = 0;
		GUILayout.BeginHorizontal();
		foreach (KeyValuePair<string, Creep> entry in pState.race.creepMap)
		{
			rowCount++;
			if (GUILayout.Button(entry.Value.name)) //use entry.Value.name, after creeps have a name defined (maybe)
			{
				if (Network.isServer)
				{
					tryCreepSpawn(entry.Value.id, Network.player);
				}
				else
				{
					networkView.RPC("tryCreepSpawn", RPCMode.Server, entry.Value.id, Network.player);
				}
			}
			if (rowCount == 4)
			{
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				rowCount = 0;
			}
		}
		GUILayout.EndHorizontal();

        //Upgrade Buttons
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Button("Upgrade Creeps");
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

	public void addCreepForPlayer(string pid, Creep c) //TODO: fix params? make it a prefab? change the method name?
    {
		List<Creep> list = creepsByArena[pid];
        if(list != null)
        {
            list.Add(c);
            c.setOwner(pid);
        }
    }
	public void addTowerForPlayer(string pid, Tower t) //TODO: fix params? make it a prefab? change the method name?
    {
		List<Tower> list = towersByPlayer[pid];
        if(list != null)
        {
            list.Add(t);
            t.setOwner(pid);
        }
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
        if(creepsByArena.ContainsKey(pid))
            return creepsByArena[pid];
        return new List<Creep>(); //TODO: Throw an exception?
    }

	public void addPlayer(NetworkPlayer player)
    {
        creepsByArena.Add(player.guid, new List<Creep>());
        towersByPlayer.Add(player.guid, new List<Tower>());
        players.Add(player.guid, new PlayerState(player));
		spawns.Add (player.guid, new SpawnerState(player));
		playerNums.Add(player.guid);
    }

    public int getPlayerNum(string pid)
    {
    	for (int i = 0; i < playerNums.Count; i++)
    	{
    		if (playerNums[i] == pid)
    			{
    				return (i + 1);
    			}
    	}

		return -1;
    }

//	void OnServerInitialized () 
//	{
//		//So client registers the server player
//		initializePlayer (Network.player);
//	}

	void OnPlayerConnected(NetworkPlayer player)
	{
		if(Network.isServer)
		{
			initializePlayer(player);
			
			foreach(NetworkPlayer NPlayer in Network.connections)
			{
				networkView.RPC("initializePlayer", player, NPlayer);

				if(NPlayer != player)
				{
					networkView.RPC ("initializePlayer", NPlayer, player);
				}
			}
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

	public void onCreepLeaked(Creep creep)
	{
		removeCreep(creep.networkView.viewID, creep.getOwner());
		networkView.RPC ("removeCreep", RPCMode.OthersBuffered, creep.networkView.viewID, creep.getOwner ());

		players[creep.getOwner ()].lives -= creep.lifeCost;
		networkView.RPC ("setLives", RPCMode.OthersBuffered, players[creep.getOwner()].lives, creep.getOwner ());

		Network.Destroy(creep.gameObject);
	}

	//RPCs

	//-----------------------------------------------------
	//Server RPCs
	//-----------------------------------------------------

	[RPC]
	void tryTowerSpawn(string towerName_, Vector3 position_, NetworkPlayer player_, NetworkMessageInfo info_)
	{
		if(!Network.isServer)
		{
			Debug.Log("Clients should not receive tryTowerSpawn RPC calls!");
			return;
		}

		Debug.Log ("tryTowerSpawn received from " + info_.sender + ", player = " + player_ + ", tower = " + towerName_ + ", position = " + position_);
		tryTowerSpawn(towerName_, position_, player_);
	}

	public void tryTowerSpawn(string towerName_, Vector3 position_, NetworkPlayer player_)
	{
		Tower t;
		PlayerState ps;
		foreach(var entry in players)
		{
			Debug.Log ("Key = " + entry.Key + ", Value = " + entry.Value);
		}

		if(null == (ps = players[player_.guid])) {Debug.Log ("Player " + player_ + " does not exist!"); return;}
		foreach(var entry in ps.race.towerMap)
		{
			Debug.Log ("Key = " + entry.Key + ", Value = " + entry.Value);
			Debug.Log (entry.Value.name + ", " + entry.Value.id);
		}
		if(!ps.race.towerMap.ContainsKey(towerName_))  {Debug.Log ("Player's Race cannot build a tower of type " + towerName_ + "!"); return;}
		t = ps.race.getTower(towerName_);
		if(t.cost > ps.gold) {Debug.Log ("Player does not have enough money to build this tower!"); return;}
		
		//Pseudocode for constraint checking
		Vector3 buildpos = position_;
//		Vector3 buildpos = constraintToGrid(position_);
		
		//canBuild encapsulates checking for collisions with creeps and other towers, checking that
		//the tower is inside the buildable area of the player, and that it does not block a path
		//from start to finish
		
		//if(!canBuild(t, buildpos) {Debug.Log("Cannot build tower at this location!"); return;}
		ps.gold -= t.cost;

		t = ((GameObject)Network.Instantiate(t.prefab, buildpos, Quaternion.identity, 0)).GetComponent<Tower>();

		if(!(Network.player == player_))
			networkView.RPC("setGold", player_, ps.gold, player_.guid);
		
		//Add tower to tower lists
		addTower (t.networkView.viewID, player_.guid);
		networkView.RPC ("addTower", RPCMode.OthersBuffered, t.networkView.viewID, player_.guid);
	}

	[RPC]
	void tryCreepSpawn(string creepName_, NetworkPlayer player_, NetworkMessageInfo info_)
	{
		if(!Network.isServer)
		{
			Debug.Log("Clients should not receive tryTowerSpawn RPC calls!");
			return;
		}

		Debug.Log ("tryCreepSpawn received from " + info_.sender + ", player = " + player_ + ", creep = " + creepName_);
		tryCreepSpawn(creepName_, player_);
	}

	public void tryCreepSpawn(string creepName_, NetworkPlayer player_)
	{
		Creep c;
		PlayerState ps;
		if(null == (ps = players[player_.guid])) {Debug.Log ("Player " + player_ + " does not exist!"); return;}
		if(null == ps.race.creepMap.ContainsKey(creepName_)) {Debug.Log ("Player's Race cannot build a creep of type " + creepName_ + "!"); return;}
		c = ps.race.getCreep(creepName_);
		if(c.cost > ps.gold) {Debug.Log ("Player does not have enough money to build this creep!"); return;}
		
		/*SpawnerState ss = spawns[player_.guid];
		UnitSpawn us;
		if(!ss.hasSpawn(creepName_)) {Debug.Log ("Player's Spawner does not have creeps of this type!"); return;}
		us = ss.getSpawn(creepName_);
		if(us.currentStock == 0) {Debug.Log("Player's Spawner does not have enough stock!"); return;}
*/
		//Spawn creep and set destination afterwards
		foreach(PlayerState pstate in players.Values)
		{
			NetworkPlayer player = pstate.player;
			if(player == player_) continue;

			c = ((GameObject)Network.Instantiate(c.prefab, spawnLocation, Quaternion.identity, 0)).GetComponent<Creep>();

			ps.gold -= c.cost;

			if(!(Network.player == player_))
				networkView.RPC("setGold", player_, ps.gold, player_.guid);
			
			//Increase player income
			ps.income += c.bounty;

			if(!(Network.player == player_))
				networkView.RPC ("setIncome", player_, ps.income, player_.guid);
			
			//Add creep to creep lists, however it is we do it
			addCreep(c.networkView.viewID, player.guid);
			networkView.RPC ("addCreep", RPCMode.OthersBuffered, c.networkView.viewID, player.guid);

			c.updateDestination();
		}
		/*
		if(us.currentStock == us.maxStock)
		{
			us.lastRestock = time;

			if(!(Network.player == player_))
				networkView.RPC("setStockTimer", player_, us.lastRestock, player_.guid, creepName_);
		}
		
		us.currentStock -= 1;

		if(!(Network.player == player_))
			networkView.RPC ("setStock", player_, us.currentStock, player_.guid, creepName_);
		*/
	}

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
		spawns.Add(player.guid, new SpawnerState(player));
		playerNums.Add(player.guid);
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

	public void removeTower(NetworkViewID networkViewID, string ownerGUID_)
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
	
	public void removeCreep(NetworkViewID networkViewID, string ownerGUID_)
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

	//-----------------------------------------------------
	//SpawnerState setters
	//-----------------------------------------------------


	//setStock
	[RPC]
	void setStock(int stock_, string guid_, string creepName_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive setStock RPC calls!");
			return;
		}

		Debug.Log ("setStock received from " + info.sender + ", stock = " + stock_ + ", player = " + guid_ + ", creepName = " + creepName_);
		setStock (stock_, guid_, creepName_);
	}

	void setStock(int stock_, string guid_, string creepName_)
	{
		spawns[guid_].getSpawn(creepName_).currentStock = stock_;
	}

	//setStockTimer
	[RPC]
	void setStockTimer(float lastTime_, string guid_, string creepName_, NetworkMessageInfo info)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive setStockTimer RPC calls!");
			return;
		}
		
		Debug.Log ("setStockTimer received from " + info.sender + ", lastRestock = " + lastTime_ + ", player = " + guid_ + ", creepName = " + creepName_);
		setStockTimer (lastTime_, guid_, creepName_);
	}

	void setStockTimer(float lastTime_, string guid_, string creepName_)
	{
		spawns[guid_].getSpawn(creepName_).lastRestock = lastTime_;
	}

}
