﻿using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    private static GameState instance = null;

    Dictionary<string, List<Creep>> creepsByArena;
    //NOTE: Returns the creeps in a player's arena. Indexed by pid

    Dictionary<string, List<Tower>> towersByPlayer;
    //NOTE: Returns towers owned by a player. Indexed by pid.

    Dictionary<string, PlayerState> players;

    Dictionary<string, SpawnerState> spawns;

    List<string> playerNums;

    float incomeTimeIncrement = 10;
    float nextIncomeTime = 30;
    float time = 0;

	bool gameOver = false;
	NetworkPlayer loser;

    bool showMenu = false;

    public GameObject spawnLocation;

    public PlacementManager pMan;
    public Camera mainCamera;
    public RaceManager raceMan;
    public PathingManager pathMan;

    public GameObject player1Terrain;
    public GameObject player2Terrain;

    public TextMesh incomeTimer;

    public Font globalFont;
    public GameObject bountyPrefab;

    public GUISkin skin;

	public Color tintColor;

	public ShowSelected selection;

  	public Texture hpBar_texture;

    public static GameState getInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
        }
        return instance;
    }

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
        spawnLocation = GameObject.FindGameObjectWithTag("SpawnLocation");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        raceMan = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();
        pathMan = GetComponent<PathingManager>();
        incomeTimer = GameObject.FindGameObjectWithTag("IncomeTimer").GetComponent<TextMesh>();
        incomeTimer.gameObject.GetComponent<ParticleSystem>().Stop();
        playerNums = new List<string>();

		Vector3 pos = mainCamera.transform.position;
		Quaternion quat = mainCamera.transform.rotation;
        if (Network.isServer)
        {
            player2Terrain.tag = "Unbuildable";
        }
        else
        {
			Debug.Log("I am a client");
            mainCamera.transform.position = new Vector3(-pos.x, pos.y, pos.z);
			mainCamera.transform.Rotate(120, 180 ,0);
            incomeTimer.gameObject.transform.Rotate(Vector3.up, 180.0f);
            player1Terrain.tag = "Unbuildable";
        }

        StartCoroutine("decrementTimer", nextIncomeTime - getGameTime());
    }

    void OnLevelWasLoaded(int level)
    {
//        Awake();  //maybe just copy code from awake
    }

    void Start()
    {
        initializePlayer(Network.player);
        foreach (NetworkPlayer player in Network.connections)
        {
            initializePlayer(player);
        }
    }

    void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        if (Network.isServer)
        {
            //Update PlayerStates	
            if (time >= nextIncomeTime)
            {
                foreach (var p in players)
                {
                    PlayerState ps = p.Value;
                    ps.gold += ps.income;

                    //We know we're the server, so if the player we're updating isn't us, go ahead and RPC
                    if (ps.player != Network.player)
                        networkView.RPC("setGold", ps.player, ps.gold, ps.player);
                }
                nextIncomeTime += incomeTimeIncrement;

                StartCoroutine("decrementTimer", nextIncomeTime - getGameTime());

                networkView.RPC("setTime", RPCMode.Others, time);
                //Update everyone else's timer.  We don't want to have to rely on messages passed back in,
                //	in case a User can fabricate setIncomeTimer messages from the server itself
                networkView.RPC("setIncomeTimer", RPCMode.Others, nextIncomeTime);
            }


            foreach (var race in raceMan.raceMapValues)
            {
                foreach (var player in race.playerUnitSpawnMap.Keys)
                {
                    foreach (var us in race.playerUnitSpawnMap[player])
                    {
                        var u = us.Value;

                        if (u.currentStock == u.maxStock) continue;
                        if (time < u.initialStockTime) continue;

                        if (time >= (u.lastRestock + u.restockTime))
                        {
                            u.currentStock += 1;
                            u.lastRestock = time;

                            //If it's not us
                            if (player != Network.player.guid)
                            {
                                networkView.RPC("setStock", players[player].player, u.currentStock, players[player].player, us.Key);
                                networkView.RPC("setStockTimer", players[player].player, u.lastRestock, players[player].player, us.Key);
                            }
                        }
                    }
                }
            }

            ////Update SpawnerStates
            //foreach(var s in spawns)
            //{
            //  SpawnerState ss = s.Value;
            //  NetworkPlayer p = ss.getOwner();

            //  foreach(var k in ss.getKeys())
            //  {
            //    //Need to know key, for RPC calls
            //    var u = ss.getSpawn(k);

            //    if(u.currentStock == u.maxStock) continue;

            //    if(time < u.initialStockTime) continue;

            //    if(time >= (u.lastRestock + u.restockTime))
            //    {
            //      u.currentStock += 1;
            //      u.lastRestock = time;

            //      //If it's not us
            //      if(p != Network.player)
            //      {
            //        networkView.RPC ("setStock", p, u.currentStock, p, k);
            //        networkView.RPC ("setStockTimer", p, u.lastRestock, p, k);
            //      }
            //    }
            //  }
            //}

        }
    }

    void OnGUI()
    {
		if(gameOver)
		{
			GUI.skin = skin;
			GUILayout.BeginArea(new Rect(5, 5, Screen.width - 10, Screen.height - 10));
			GUILayout.BeginVertical("window");
			GUILayout.FlexibleSpace();
			GUILayout.Label(NameDatabase.getName(loser.guid) + " has lost!");
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.EndArea();
			return;
		}


		GUISkin oldskin = GUI.skin;

		GUI.skin = skin;

        GUILayout.BeginArea(new Rect(5, 5, Screen.width - 10, Screen.height - 10));
        GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
        OnGUI_TopBar();
		GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        OnGUI_TowerBar();
        OnGUI_CreepBar();

        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (showMenu)
        {
            GUILayout.Window(0, new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400, 400), WindowGUI, "", GUILayout.Width(400), GUILayout.Height(400));
        }

		GUI.skin = oldskin;
		if(GUI.tooltip != "")
		{
			var x = Event.current.mousePosition.x;
			var y = Event.current.mousePosition.y;
			
			GUI.Label(new Rect(x - 150, y + 20, 160, 90), GUI.tooltip, "box");
		}
    }

    void WindowGUI(int windowID)
    {
        GUILayout.BeginVertical();
        GUILayout.Label("Menu");
        if (GUILayout.Button("Back"))
        {
            showMenu = false;
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Main Menu"))
        {
            if (Network.isServer)
            {
                networkView.RPC("endGame", RPCMode.OthersBuffered);
            }
            endGame();
        }

        if (GUILayout.Button("Quit Game"))
        {
            if (Network.isServer)
            {
                networkView.RPC("endGame", RPCMode.OthersBuffered);
            }

            endGame();
            Application.Quit();
        }
        GUILayout.EndVertical();
    }

    void OnGUI_TopBar()
    {
		GUILayout.BeginArea(new Rect(0, -35, Screen.width, 75));
		GUILayout.BeginVertical();
//		GUILayout.FlexibleSpace();


        GUILayout.BeginHorizontal("window");

        if (GUILayout.Button("Menu", GUILayout.Width (200)))
        {
            showMenu = true;
        }

        GUILayout.FlexibleSpace();

		GUILayout.Label(NameDatabase.getName(Network.player.guid) + "'s Lives: " + players[Network.player.guid].lives);

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
		Color last = GUI.color;
		GUI.color = Color.yellow;
		GUILayout.Label("Gold: " + currGold);
		GUI.color = Color.green;
		GUILayout.Label("Income: " + currIncome);
		GUI.color = last;

		foreach (var p in players)
		{
			if (p.Value.player == Network.player) continue;
			GUILayout.Label (NameDatabase.getName(p.Value.player.guid) + "'s Lives: " + p.Value.lives);
		}

		if(players.Count == 1)
		{
			GUILayout.FlexibleSpace();
		}

        GUILayout.FlexibleSpace();

		GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();


		GUILayout.EndVertical();
		GUILayout.EndArea();
    }

    void OnGUI_ScoreBoard()
    {
        int friendlyLives = 0;
        int opponentLives = 0;

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        string scoreBoardString = NameDatabase.getName(Network.player.guid) + "'s Lives: " + players[Network.player.guid].lives;

        foreach (var p in players)
        {
            if (p.Value.player == Network.player) continue;
            scoreBoardString += "\n" + NameDatabase.getName(p.Value.player.guid) + "'s Lives: " + p.Value.lives;
        }

        GUILayout.Box(scoreBoardString);

        GUILayout.EndHorizontal();
    }

    void OnGUI_TowerBar()
    {
        PlayerState pState = players[Network.player.guid];

        GUILayout.BeginArea(new Rect(0, 10, 275, Screen.height - 20));

        //TOWER PLACEMENT BUTTONS
        GUILayout.BeginVertical("window");
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Towers");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        foreach (KeyValuePair<string, Tower> entry in pState.race.towerMap.Reverse())
        {
            GUILayout.BeginVertical("box");
            Color oldColor = GUI.color;
            bool canUse = entry.Value.cost <= pState.gold;
            if (!canUse)
            {
                GUI.color = Color.red;
                GUI.enabled = false;
            }

            if (GUILayout.Button(new GUIContent(entry.Key, entry.Value.getDescription()))) //use entry.Value.name after towers have a name defined (maybe)
            {
                pMan.enabled = true;
                pMan.beginPlacing(entry.Value.prefab, entry.Key);
            }
            GUI.enabled = true;
            GUI.color = Color.yellow;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(entry.Value.cost + " Gold");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.color = oldColor;
            GUILayout.EndVertical();
        }

        OnGUI_StatusBarTower();

        GUILayout.EndArea();
    }

    void OnGUI_StatusBarTower()
    {
        GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

		if(selection.selected != null && (selection.selected.getOwner() == Network.player.guid))
		{
			GUILayout.Label(selection.selected.name);

			GUILayout.BeginVertical("box");
			GUILayout.Label("Upgrade");

			if(selection.selected.upgrade != null)
			{
				int cost = selection.selected.upgrade.cost - selection.selected.cost;
				PlayerState pstate = players[Network.player.guid];
				Color last = GUI.color;
				if(cost > pstate.gold)
				{
					GUI.color = Color.red;
					GUI.enabled = false;
				}

	        	if (GUILayout.Button(selection.selected.upgrade.name))
	        	{
					if(Network.isServer)
					{
						tryUpgradeTower(selection.selected.networkView.viewID, Network.player);
					}
					else
					{
						networkView.RPC ("tryUpgradeTower", RPCMode.Server, selection.selected.networkView.viewID);
					}
	        	}
				GUI.enabled = true;
				GUI.color = Color.yellow;
				GUILayout.Label(cost.ToString() + " Gold");
				GUI.color = last;
			}
			else
			{
				GUI.enabled = false;
				GUILayout.Button("This tower cannot upgrade!");
				GUI.enabled = true;
			}
			GUILayout.EndVertical();
	        GUILayout.FlexibleSpace();

	        if (GUILayout.Button("Sell for " + (int)(selection.selected.cost * .7) + " Gold."))
	        {
				if(Network.isServer)
				{
					trySellTower(selection.selected.networkView.viewID, Network.player);
				}
				else
				{
					networkView.RPC("trySellTower", RPCMode.Server, selection.selected.networkView.viewID);
				}
	        }
		}

        GUILayout.EndVertical();
    }

    void OnGUI_CreepBar()
    {
		float width = 350;
        GUILayout.BeginArea(new Rect(Screen.width - (width + 5), 10, width, Screen.height - 20));
        PlayerState pState = players[Network.player.guid];

        GUILayout.BeginVertical("window", GUILayout.ExpandWidth(true));
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Creeps");
		GUILayout.Label(pState.creepUpgradeLevel.ToString());
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        foreach (KeyValuePair<string, Creep> entry in pState.race.creepMap.Reverse())
        {
            GUILayout.BeginHorizontal();

			var usm = players[Network.player.guid].race.getUnitSpawnMap(Network.player.guid);
			UnitSpawn us = usm[entry.Key];

            Color oldColor = GUI.color;
            bool canUse = us.cost <= pState.gold;
            if (!canUse)
            {
                GUI.color = Color.red;
                GUI.enabled = false;
            }


            //UnitSpawn us = pState.race.getUnitSpawn(entry.Key);


            if (us.currentStock <= 0)
            {
                GUI.enabled = false;
            }

			int health = (int)entry.Value.healthBase;
			for(int i = 0; i < pState.creepUpgradeLevel; i++)
			{
				health *= 2;
			}

			string tooltip = entry.Value.name + '\n' + "Gold: " + us.cost+ "\t" + "Bounty: " + us.income + "\n" + "Health: " + health + "\n" + "Speed: " + entry.Value.getSpeed();

            if (GUILayout.Button(new GUIContent(entry.Key.Replace(' ', '\n'), tooltip), GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true), GUILayout.MinWidth(100))) //use entry.Value.name, after creeps have a name defined (maybe)
            {
                if (Network.isServer)
                {
                    tryCreepSpawn(entry.Key, Network.player);
                }
                else
                {
                    networkView.RPC("tryCreepSpawn", RPCMode.Server, entry.Key, Network.player);
                }
            }
            GUI.enabled = true;

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUI.color = Color.yellow;
            GUILayout.Label(us.cost.ToString()/* + " Gold"*/);
            GUI.color = Color.green;
            GUILayout.Label("+" + us.income.ToString()/* + " Income"*/);
            GUILayout.EndHorizontal();

            if (us == null) { Debug.Log("Not UnitSpawn found for creep: " + entry.Key + "!"); }
            else
            {
                if (us.currentStock > 0)
                    GUI.color = Color.white;
                else
                    GUI.color = Color.red;

                if (time < us.initialStockTime)
                    GUILayout.Label("Stock: " + us.currentStock + " / " + us.maxStock + " : " + (int)(us.initialStockTime - time));
                else
					GUILayout.Label("Stock: " + us.currentStock + " / " + us.maxStock + " : " + ((us.currentStock == us.maxStock) ? "-" : ((int)(((time < us.initialStockTime) ? us.initialStockTime : us.lastRestock + us.restockTime) - time )).ToString()));
            }
            GUI.color = oldColor;

			GUILayout.Label("", "Divider");//-------------------------------- custom
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }


        //Upgrade Buttons
        GUILayout.FlexibleSpace();
		GUILayout.BeginVertical("box");

		int cost = 500;
		for(int i = 0; i < pState.creepUpgradeLevel; i++)
		{
			cost *= 2;
		}

		Color last = GUI.color;

		if(cost > pState.gold)
		{
			GUI.color = Color.red;
			GUI.enabled = false;
		}

        if(GUILayout.Button("Upgrade Creeps", GUILayout.ExpandWidth(true)))
		{
			if(Network.isServer)
			{
				tryUpgradeCreeps(Network.player);
			}
			else
			{
				networkView.RPC ("tryUpgradeCreeps", RPCMode.Server);
			}
		}
		GUI.enabled = true;

		GUI.color = Color.yellow;
		GUILayout.Label(cost.ToString());
		GUI.color = last;

        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.EndArea();
    }
//
//    void OnGUI_BottomBar()
//    {
//        GUILayout.BeginHorizontal();
//
//        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3), GUILayout.Height(Screen.height / 5));
//        GUILayout.Label("Buffs: ");
//        GUILayout.EndVertical();
//
//        //GUILayout.FlexibleSpace();
//
//        //Used to set player's towers and creeps
//        PlayerState pState = players[Network.player.guid];
//        int rowCount = 0;
//
//        //TOWER PLACEMENT BUTTONS
//        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3), GUILayout.Height(Screen.height / 5));
//        GUILayout.BeginHorizontal();
//        GUILayout.FlexibleSpace();
//        GUILayout.Label("Towers");
//        GUILayout.FlexibleSpace();
//        GUILayout.EndHorizontal();
//        GUILayout.BeginHorizontal();
//        foreach (KeyValuePair<string, Tower> entry in pState.race.towerMap.Reverse())
//        {
//            rowCount++;
//            if (GUILayout.Button(entry.Key + "\n" + entry.Value.cost + "G")) //use entry.Value.name after towers have a name defined (maybe)
//            {
//                pMan.enabled = true;
//                pMan.beginPlacing(entry.Value.prefab, entry.Key);
//            }
//            if (rowCount == 4)
//            {
//                GUILayout.EndHorizontal();
//                GUILayout.BeginHorizontal();
//                rowCount = 0;
//            }
//        }
//        GUILayout.EndHorizontal();
//
//        GUILayout.EndVertical();
//
//        //GUILayout.FlexibleSpace();
//
//        //CREEP PLACEMENT BUTTONS
//        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 3 - 30), GUILayout.Height(Screen.height / 5));
//        GUILayout.BeginHorizontal();
//        GUILayout.FlexibleSpace();
//        GUILayout.Label("Creeps");
//        GUILayout.FlexibleSpace();
//        GUILayout.EndHorizontal();
//        rowCount = 0;
//        GUILayout.BeginHorizontal();
//        foreach (KeyValuePair<string, Creep> entry in pState.race.creepMap)
//        {
//            GUILayout.BeginVertical("box");
//
//            rowCount++;
//            if (GUILayout.Button(entry.Key + "\n" + entry.Value.cost + "G")) //use entry.Value.name, after creeps have a name defined (maybe)
//            {
//                if (Network.isServer)
//                {
//                    tryCreepSpawn(entry.Key, Network.player);
//                }
//                else
//                {
//                    networkView.RPC("tryCreepSpawn", RPCMode.Server, entry.Key, Network.player);
//                }
//            }
//
//            //UnitSpawn us = pState.race.getUnitSpawn(entry.Key);
//            var usm = players[Network.player.guid].race.getUnitSpawnMap(Network.player.guid);
//            UnitSpawn us = usm[entry.Key];
//            if (us == null) { Debug.Log("Not UnitSpawn found for creep: " + entry.Key + "!"); }
//            else
//            {
//                if (time < us.initialStockTime)
//                    GUILayout.Label("Stock: " + us.currentStock + " / " + us.maxStock + " : " + (int)(us.initialStockTime - time));
//                else
//                    GUILayout.Label("Stock: " + us.currentStock + " / " + us.maxStock + " : " + (int)(us.restockTime - ((time - us.initialStockTime) % us.restockTime)));
//            }
//
//            GUILayout.EndVertical();
//            if (rowCount == 4)
//            {
//                GUILayout.EndHorizontal();
//                GUILayout.BeginHorizontal();
//                rowCount = 0;
//            }
//        }
//        GUILayout.EndHorizontal();
//
//        //Upgrade Buttons
//        GUILayout.FlexibleSpace();
//        GUILayout.BeginHorizontal();
//        GUILayout.FlexibleSpace();
//        GUILayout.Button("Upgrade Creeps");
//        GUILayout.EndHorizontal();
//        GUILayout.EndVertical();
//
//        GUILayout.EndHorizontal();
//    }

    public Vector3 getEndPoint()
    {
        GameState gstate = GameState.getInstance();
        if (Network.isServer)
        {
            return new Vector3(43.75f, .5f, 0);
        }
        else
        {
            return new Vector3(-43.75f, .5f, 0);
        }
    }


    public NetworkPlayer getPlayer(string pid)
    {
        return players[pid].player;
    }

    public void addCreepForPlayer(string pid, Creep c) //TODO: fix params? make it a prefab? change the method name?
    {
        List<Creep> list = creepsByArena[pid];
        if (list != null)
        {
            list.Add(c);
            c.setOwner(pid);
        }
    }
    public void addTowerForPlayer(string pid, Tower t) //TODO: fix params? make it a prefab? change the method name?
    {
        List<Tower> list = towersByPlayer[pid];
        if (list != null)
        {
            list.Add(t);
            t.setOwner(pid);
        }
    }

    public void removeCreepForPlayer(string pid, Creep c)
    {
        List<Creep> list = creepsByArena[pid];
        if (list != null)
        {
            if (list.Contains(c))
                list.Remove(c);
        }
    }

    public void removeTowerForPlayer(string pid, Tower t)
    {
        List<Tower> list = towersByPlayer[pid];
        if (list != null)
            list.Remove(t);
    }

    public List<Creep> getEnemyCreeps(string pid)
    {
        if (creepsByArena.ContainsKey(pid))
            return creepsByArena[pid];
        return new List<Creep>(); //TODO: Throw an exception?
    }
    public List<Tower> getPlayerTowers(string pid)
    {
        if(towersByPlayer.ContainsKey(pid))
            return towersByPlayer[pid];
        return new List<Tower>();
    }

    public void addPlayer(NetworkPlayer player)
    {
        creepsByArena.Add(player.guid, new List<Creep>());
        towersByPlayer.Add(player.guid, new List<Tower>());
        players.Add(player.guid, new PlayerState(player, raceMan.raceMap["Undead"]));
        spawns.Add(player.guid, new SpawnerState(player));
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
        if (Network.isServer)
        {
            initializePlayer(player);

            foreach (NetworkPlayer NPlayer in Network.connections)
            {
                networkView.RPC("initializePlayer", player, NPlayer);

                if (NPlayer != player)
                {
                    networkView.RPC("initializePlayer", NPlayer, player);
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

		if(Network.isClient) return;

        var ps = players[creep.getOwner()];

        removeCreep(creep.networkView.viewID, ps.player);
        networkView.RPC("removeCreep", RPCMode.Others, creep.networkView.viewID, ps.player);

        if ((ps.lives -= creep.lifeCost) <= 0)
        {
            networkView.RPC("gameLost", RPCMode.Others, ps.player);
            gameLost(ps.player);
        }
        networkView.RPC("setLives", RPCMode.Others, ps.lives, ps.player);

        Network.Destroy(creep.gameObject);
    }

    public void onCreepDeath(Creep creep)
    {
		NetworkPlayer owner = getPlayer (creep.getOwner());
        removeCreep(creep.networkView.viewID, owner);
        networkView.RPC("removeCreep", RPCMode.Others, creep.networkView.viewID, owner);

        var ps = players[creep.getOwner()];
        ps.gold += creep.bounty;

        if (ps.player != Network.player)
        {
            networkView.RPC("setGold", ps.player, ps.gold, ps.player);
        }

		Vector3 pos = creep.transform.position;
		int bounty = creep.getBounty();

		if(Network.player == owner)
		{
			showCreepGold(pos, bounty);
		}
		else
		{
			networkView.RPC ("showCreepGold", owner, pos, bounty);
		}

        Network.Destroy(creep.gameObject);
    }

    //Co-routines

    //Income Timer Text Helper

    IEnumerator decrementTimer(int i)
    {
        while (i > 0)
        {
            incomeTimer.text = i.ToString();
            i--;
            yield return new WaitForSeconds(1f);
        }

        incomeTimer.text = i.ToString();
        StartCoroutine("incomeParticles");
    }

    IEnumerator incomeParticles()
    {
        incomeTimer.gameObject.GetComponent<ParticleSystem>().Play();
        incomeTimer.gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2.5f);
        incomeTimer.gameObject.GetComponent<ParticleSystem>().Stop();
    }

    IEnumerator creepBountyEffect(TextMesh creepBounty)
    {
        var target_time = time + 1.5f;
        var sway = 0f;
        while (time < target_time)
        {
            sway = (((target_time - time) / 0.25f) % 2 > 1) ? -1.0f : 1.0f;
            creepBounty.gameObject.transform.position += new Vector3(0, 2f * Time.deltaTime, sway * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Destroy(creepBounty.gameObject);
    }

    //RPCs

    //-----------------------------------------------------
    //Server RPCs
    //-----------------------------------------------------

    [RPC]
    void tryTowerSpawn(string towerName_, Vector3 position_, NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (!Network.isServer)
        {
            Debug.Log("Clients should not receive tryTowerSpawn RPC calls!");
            return;
        }

        Debug.Log("tryTowerSpawn received from " + info_.sender + ", player = " + player_ + ", tower = " + towerName_ + ", position = " + position_);
        tryTowerSpawn(towerName_, position_, player_);
    }

    public void tryTowerSpawn(string towerName_, Vector3 position_, NetworkPlayer player_)
    {
        Tower t;
        PlayerState ps;
        foreach (var entry in players)
        {
            Debug.Log("Key = " + entry.Key + ", Value = " + entry.Value);
        }

        if (null == (ps = players[player_.guid])) { Debug.Log("Player " + player_ + " does not exist!"); return; }
        foreach (var entry in ps.race.towerMap)
        {
            Debug.Log("Key = " + entry.Key + ", Value = " + entry.Value);
            Debug.Log(entry.Value.name + ", " + entry.Key);
        }
        if (!ps.race.towerMap.ContainsKey(towerName_)) { Debug.Log("Player's Race cannot build a tower of type " + towerName_ + "!"); return; }
        t = ps.race.getTower(towerName_);
        if (t.cost > ps.gold) { Debug.Log("Player does not have enough money to build this tower!"); return; }

        //Pseudocode for constraint checking
        Vector3 buildpos = position_;
        //		Vector3 buildpos = constraintToGrid(position_);

        //canBuild encapsulates checking for collisions with creeps and other towers, checking that
        //the tower is inside the buildable area of the player, and that it does not block a path
        //from start to finish

        //if(!canBuild(t, buildpos) {Debug.Log("Cannot build tower at this location!"); return;}
        ps.gold -= t.cost;

        t = ((GameObject)Network.Instantiate(t.prefab, buildpos, t.prefab.transform.rotation, 0)).GetComponent<Tower>();

        if (!(Network.player == player_))
            networkView.RPC("setGold", player_, ps.gold, player_);

        //Add tower to tower lists
        addTower(t.networkView.viewID, player_);
        networkView.RPC("addTower", RPCMode.Others, t.networkView.viewID, player_);
    }

    [RPC]
    void tryCreepSpawn(string creepName_, NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (!Network.isServer)
        {
            Debug.Log("Clients should not receive tryTowerSpawn RPC calls!");
            return;
        }

        Debug.Log("tryCreepSpawn received from " + info_.sender + ", player = " + player_ + ", creep = " + creepName_);
        tryCreepSpawn(creepName_, player_);
    }

    public void tryCreepSpawn(string creepName_, NetworkPlayer player_)
    {
        Creep c;
        PlayerState ps;
        if (null == (ps = players[player_.guid])) { Debug.Log("Player " + player_ + " does not exist!"); return; }
        if (!ps.race.creepMap.ContainsKey(creepName_)) { Debug.Log("Player's Race cannot build a creep of type " + creepName_ + "!"); return; }

        /*SpawnerState ss = spawns[player_.guid];
        UnitSpawn us;
        if(!ss.hasSpawn(creepName_)) {Debug.Log ("Player's Spawner does not have creeps of this type!"); return;}
        us = ss.getSpawn(creepName_);
        if(us.currentStock == 0) {Debug.Log("Player's Spawner does not have enough stock!"); return;}
    */

        UnitSpawn us;
        if (null == (us = players[player_.guid].race.getUnitSpawnMap(player_.guid)[creepName_]))
        {
            Debug.Log("Player's Race UnitSpawnMap does not have creeps of type " + creepName_ + "!");
            return;
        }
        if (us.currentStock == 0)
        {
            Debug.Log("Player's Spawner does not have enough stock!");
            return;
        }

		c = ps.race.getCreep(creepName_);
		if (us.cost > ps.gold) { Debug.Log("Player does not have enough money to build this creep!"); return; }

        //Spawn creep and set destination afterwards
        foreach (PlayerState pstate in players.Values)
        {
            NetworkPlayer player = pstate.player;
            if (player == player_ && players.Count > 1) continue;

            c = ((GameObject)Network.Instantiate(c.prefab, player == Network.player ? pathMan.player1Spawn.getPos() : pathMan.player2Spawn.getPos(), Quaternion.identity, 0)).GetComponent<Creep>();

			int health = (int)c.getHealth();
			for(int i = 0; i < pstate.creepUpgradeLevel; i++)
			{
				health *= 2;
			}

			c.setHealth(health);
			c.networkView.RPC("setHealthRPC", RPCMode.Others, health);

			c.setBounty(us.income);
			c.networkView.RPC("setBountyRPC", RPCMode.Others, us.income);

            ps.gold -= us.cost;

            if (!(Network.player == player_))
                networkView.RPC("setGold", player_, ps.gold, player_);

            //Increase player income
            ps.income += us.income;

            if (!(Network.player == player_))
                networkView.RPC("setIncome", player_, ps.income, player_);

            //Add creep to creep lists, however it is we do it
            addCreep(c.networkView.viewID, player);
            networkView.RPC("addCreep", RPCMode.Others, c.networkView.viewID, player);

            c.gameObject.GetComponent<PathingAgent>().nextNode = c.getOwner() == Network.player.guid ? pathMan.player1Spawn : pathMan.player2Spawn;
            c.gameObject.GetComponent<PathingAgent>().grid = c.getOwner() == Network.player.guid ? pathMan.player1Zone : pathMan.player2Zone;
        }

        if (us.currentStock == us.maxStock)
        {
            us.lastRestock = time;
            if (!(Network.player == player_))
                networkView.RPC("setStockTimer", player_, us.lastRestock, player_.guid, creepName_);
        }

        us.currentStock -= 1;

        if (!(Network.player == player_))
            networkView.RPC("setStock", player_, us.currentStock, player_.guid, creepName_);


        //old stock code
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

	[RPC]
	void trySellTower(NetworkViewID id, NetworkMessageInfo info_)
	{
		if(Network.isClient)
		{
			Debug.Log ("Client should not receive trySellTower RPC Calls!");
			return;
		}

		trySellTower(id, info_.sender);
	}

	void trySellTower(NetworkViewID id, NetworkPlayer player)
	{
		GameObject obj = NetworkView.Find(id).gameObject;
		Tower t = obj.GetComponent<Tower>();

		if(t.getOwner() != player.guid)
		{
			Debug.Log ("Owning player is not sender of sell call!");
			return;
		}

		PlayerState ps = players[player.guid];
		ps.gold += (int)(t.cost*.7);

		removeTower(obj.networkView.viewID, getPlayer (t.getOwner()));
		networkView.RPC ("removeTower", RPCMode.Others, obj.networkView.viewID, t.getOwner());
		
		//We know we're the server, so if the player we're updating isn't us, go ahead and RPC
		if (ps.player != Network.player)
			networkView.RPC("setGold", ps.player, ps.gold, ps.player);

		Network.Destroy(obj);
	}

	[RPC]
	void tryUpgradeTower(NetworkViewID id, NetworkMessageInfo info_)
	{
		if(Network.isClient)
		{
			Debug.Log("Client should not receive tryUpgradeTower RPC Calls!");
			return;
		}

		tryUpgradeTower (id, info_.sender);
	}

	void tryUpgradeTower(NetworkViewID id, NetworkPlayer player)
	{
		GameObject obj = NetworkView.Find(id).gameObject;
		Tower t = obj.GetComponent<Tower>();

		if(t.getOwner() != player.guid)
		{
			Debug.Log ("Owning player is not sender of upgrade call!");
			return;
		}

		Tower upgrade = t.upgrade;

		if(upgrade == null)
		{
			Debug.Log ("Tower has no upgrade.");
			return;
		}

		int cost = upgrade.cost - t.cost;

		PlayerState ps = players[player.guid];

		if(ps.gold < cost)
		{
			Debug.Log ("Player does not have enough money!");
			return;
		}

		GameObject prefab = upgrade.prefab;

		Vector3 pos = t.transform.position;

		removeTower(t.networkView.viewID, player);
		networkView.RPC ("removeTower", RPCMode.Others, t.networkView.viewID, player);
		Network.Destroy(obj);
		
		ps.gold -= cost;

		t = ((GameObject)Network.Instantiate(prefab, pos, prefab.transform.rotation, 0)).GetComponent<Tower>();
		
		if (!(Network.player == player))
			networkView.RPC("setGold", player, ps.gold, player);
		
		//Add tower to tower lists
		addTower(t.networkView.viewID, player);
		networkView.RPC("addTower", RPCMode.Others, t.networkView.viewID, player);
	}

	[RPC]
	void tryUpgradeCreeps(NetworkMessageInfo info_)
	{
		if(Network.isClient)
		{
			Debug.Log("Client should not receive tryUpgradeCreeps RPCs!");
			return;
		}

		tryUpgradeCreeps(info_.sender);
	}

	void tryUpgradeCreeps(NetworkPlayer player)
	{
		int cost = 500;

		PlayerState pstate = players[player.guid];

		for(int i = 0; i < pstate.creepUpgradeLevel; i++)
		{
			cost *= 2;
		}

		if(cost > pstate.gold)
		{
			Debug.Log ("Player does not have enough gold to upgrade creeps!");
			return;
		}

		pstate.gold -= cost;
		pstate.creepUpgradeLevel += 1;
		upgradeCreepLevel(player);

		if(player != Network.player)
		{
			networkView.RPC("setGold", player, pstate.gold, player);
			networkView.RPC("setCreepUpgradeLevel", player, pstate.creepUpgradeLevel, player);
			networkView.RPC("upgradeCreepLevel", player);
		}
	}
	
	//-----------------------------------------------------
    //Client RPCs
    //-----------------------------------------------------

    [RPC]
    void endGame(NetworkMessageInfo info_)
    {

        if (Network.isServer)
        {
            Debug.Log("Server should not receive endGame RPC calls!");
            return;
        }
        endGame();
    }

    void endGame()
    {
        Network.Disconnect();
        Application.LoadLevel("MainScene");
    }

	[RPC]
	void gameLost(NetworkPlayer loser_, NetworkMessageInfo info_)
	{
		gameLost(loser_);
	}

	void gameLost(NetworkPlayer loser_)
	{
		if(gameOver) return;
		
		gameOver = true;
		loser = loser_;
		StartCoroutine("endCountdown", 5f);
	}

	IEnumerator endCountdown(float time)
	{
		yield return new WaitForSeconds(time);
		endGame();
	}

	[RPC]
	void showCreepGold(Vector3 pos, int amount, NetworkMessageInfo info_)
	{
		showCreepGold(pos, amount);
	}

	void showCreepGold(Vector3 pos, int amount)
	{
		
		TextMesh creepBounty = (Instantiate(bountyPrefab, pos, Quaternion.identity) as GameObject).GetComponent<TextMesh>();
		creepBounty.gameObject.transform.position = pos + new Vector3(0, 2, 0);
		creepBounty.text = "+" + amount;
		
		float rotation;
		if (Network.isServer)
		{
			rotation = 90;
		}
		else
		{
			rotation = 270;
		}
		creepBounty.gameObject.transform.Rotate(Vector3.up, rotation);
		StartCoroutine("creepBountyEffect", creepBounty);
	}

    //-----------------------------------------------------
    //Player adding
    //-----------------------------------------------------

    [RPC]
    void initializePlayer(NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive initializePlayer RPC calls!");
            return;
        }

        Debug.Log("initialzePlayer received from " + info_.sender + ", player = " + player_);
        initializePlayer(player_, info_);
    }

    void initializePlayer(NetworkPlayer player)
    {
        players.Add(player.guid, new PlayerState(player, raceMan.raceMap[RaceDatabase.getRace(player)]));
        creepsByArena.Add(player.guid, new List<Creep>());
        towersByPlayer.Add(player.guid, new List<Tower>());
        spawns.Add(player.guid, new SpawnerState(player));
        playerNums.Add(player.guid);
    }

    //Player removing

    [RPC]
    void removePlayer(NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive removePlayer RPC calls!");
            return;
        }

        Debug.Log("removePlayer received from " + info_.sender + ", player = " + player_);
        removePlayer(player_);
    }

    void removePlayer(NetworkPlayer player_)
    {
        players.Remove(player_.guid);
        creepsByArena.Remove(player_.guid);
        towersByPlayer.Remove(player_.guid);
        spawns.Remove(player_.guid);
    }

    //-----------------------------------------------------
    //Tower and Creep List Modifiers
    //-----------------------------------------------------

    //Add Tower

    [RPC]
    void addTower(NetworkViewID networkViewID, NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive addTower RPC calls!");
            return;
        }

        Debug.Log("addTower received from " + info_.sender + ", owner = " + player_ + ", NetworkViewID = " + networkViewID);
        addTower(networkViewID, player_);
    }

    void addTower(NetworkViewID networkViewID, NetworkPlayer player_)
    {
        NetworkView view = NetworkView.Find(networkViewID);
        addTowerForPlayer(player_.guid, view.gameObject.GetComponent<Tower>());
    }

    //Add Creep

    [RPC]
    void addCreep(NetworkViewID networkViewID, NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive addCreep RPC calls!");
            return;
        }

        Debug.Log("addCreep received from " + info_.sender + ", owner = " + player_ + ", NetworkViewID = " + networkViewID);
        addCreep(networkViewID, player_);
    }

    void addCreep(NetworkViewID networkViewID, NetworkPlayer player_)
    {
        NetworkView view = NetworkView.Find(networkViewID);
        addCreepForPlayer(player_.guid, view.gameObject.GetComponent<Creep>());
    }

    //Remove Tower
    [RPC]
    void removeTower(NetworkViewID networkViewID, NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive removeTower RPC calls!");
            return;
        }

        Debug.Log("removeTower recieved from " + info_.sender + ", owner = " + player_ + ", NetworkViewID = " + networkViewID);
        removeTower(networkViewID, player_);
    }

    public void removeTower(NetworkViewID networkViewID, NetworkPlayer player_)
    {
        NetworkView view = NetworkView.Find(networkViewID);
        removeTowerForPlayer(player_.guid, view.gameObject.GetComponent<Tower>());
    }

    //Remove Creep
    [RPC]
    void removeCreep(NetworkViewID networkViewID, NetworkPlayer player_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive removeCreep RPC calls!");
            return;
        }

        Debug.Log("removeCreep recieved from " + info_.sender + ", owner = " + player_ + ", NetworkViewID = " + networkViewID);
        removeCreep(networkViewID, player_);
    }

    public void removeCreep(NetworkViewID networkViewID, NetworkPlayer player_)
    {
        NetworkView view = NetworkView.Find(networkViewID);
        removeCreepForPlayer(player_.guid, view.gameObject.GetComponent<Creep>());
    }

    //-----------------------------------------------------
    //GameState setters
    //-----------------------------------------------------

    //setTime
    [RPC]
    void setTime(float time_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive setTime RPC calls!");
            return;
        }

        Debug.Log("Received setTime from " + info_ + ", time = " + time_);
        time = time_;
    }

    //setIncomeTimer
    [RPC]
    void setIncomeTimer(float time_, NetworkMessageInfo info_)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive setIncomeTimer RPC calls!");
            return;
        }

        Debug.Log("Received setIncomeTimer from " + info_ + ", time = " + time_);
        nextIncomeTime = time_;
    }


    //-----------------------------------------------------
    //PlayerState setters
    //-----------------------------------------------------

    //setGold
    [RPC]
    void setGold(int gold_, NetworkPlayer player_, NetworkMessageInfo info)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive setGold RPC calls!");
            return;
        }
        if (Network.isClient && (player_ != Network.player))
        {
            Debug.Log("Received setGold for wrong player!");
            return;
        }
        Debug.Log("setGold received from " + info.sender + ", amount = " + gold_ + ", player = " + player_);
        setGold(gold_, player_);
    }

    void setGold(int gold_, NetworkPlayer player_)
    {
        PlayerState state = players[player_.guid];
        if (state == null) { Debug.Log("Player " + player_ + " not found!"); return; }
        state.gold = gold_;
    }

    //setIncome
    [RPC]
    void setIncome(int income_, NetworkPlayer player_, NetworkMessageInfo info)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive setIncome RPC calls!");
            return;
        }
        if (Network.isClient && (player_ != Network.player))
        {
            Debug.Log("Received setIncome for wrong player!");
            return;
        }
        Debug.Log("setIncome received from " + info.sender + ", income = " + income_ + ", player = " + player_);
        setIncome(income_, player_);
    }

    void setIncome(int income_, NetworkPlayer player_)
    {
        PlayerState state = players[player_.guid];
        if (state == null) { Debug.Log("Player " + player_ + " not found!"); return; }
        state.income = income_;
        StartCoroutine("decrementTimer", nextIncomeTime - getGameTime());
    }

    //setLives
    [RPC]
    void setLives(int lives_, NetworkPlayer player_, NetworkMessageInfo info)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive setLives RPC calls!");
            return;
        }

        Debug.Log("setLives received from " + info.sender + ", lives = " + lives_ + ", player = " + player_);
        setLives(lives_, player_);
    }

    void setLives(int lives_, NetworkPlayer player_)
    {
        PlayerState state = players[player_.guid];
        if (state == null) { Debug.Log("Player " + player_ + " not found!"); return; }
        state.lives = lives_;
    }

	[RPC]
	void setCreepUpgradeLevel(int level_, NetworkPlayer player_, NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive setCreepUpgradeLevel RPC calls!");
			return;
		}

		setCreepUpgradeLevel(level_, player_);
	}

	void setCreepUpgradeLevel(int level_, NetworkPlayer player_)
	{
		PlayerState state = players[player_.guid];
		if(state == null) {Debug.Log ("Player " + player_ + " not found!"); return; }
		state.creepUpgradeLevel = level_;
	}

	[RPC]
	void upgradeCreepLevel(NetworkMessageInfo info_)
	{
		if(Network.isServer)
		{
			Debug.Log ("Server should not receive upgradeCreepLevel RPC calls!");
			return;
		}

		upgradeCreepLevel(info_.sender);
	}

	void upgradeCreepLevel(NetworkPlayer player_)
	{
		PlayerState pstate = players[player_.guid];
		if(pstate == null) {Debug.Log ("Player " + player_ + " not found!"); return; }
		foreach(var spawn in pstate.race.getUnitSpawnMap(player_.guid))
		{
			spawn.Value.currentStock = 0;
			spawn.Value.maxStock += 1;
			spawn.Value.cost *= 2;
			spawn.Value.income *= 2;
		}
	}

    //-----------------------------------------------------
    //SpawnerState setters
    //-----------------------------------------------------


    //setStock
    [RPC]
    void setStock(int stock_, NetworkPlayer player_, string creepName_, NetworkMessageInfo info)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive setStock RPC calls!");
            return;
        }

        Debug.Log("setStock received from " + info.sender + ", stock = " + stock_ + ", player = " + player_ + ", creepName = " + creepName_);
        setStock(stock_, player_, creepName_);
    }

    void setStock(int stock_, NetworkPlayer player_, string creepName_)
    {
        //spawns[player_.guid].getSpawn(creepName_).currentStock = stock_;

        players[player_.guid].race.getUnitSpawnMap(player_.guid)[creepName_].currentStock = stock_;
    }

    //setStockTimer
    [RPC]
    void setStockTimer(float lastTime_, NetworkPlayer player_, string creepName_, NetworkMessageInfo info)
    {
        if (Network.isServer)
        {
            Debug.Log("Server should not receive setStockTimer RPC calls!");
            return;
        }

        Debug.Log("setStockTimer received from " + info.sender + ", lastRestock = " + lastTime_ + ", player = " + player_ + ", creepName = " + creepName_);
        setStockTimer(lastTime_, player_, creepName_);
    }

    void setStockTimer(float lastTime_, NetworkPlayer player_, string creepName_)
    {
        //spawns[player_.guid].getSpawn(creepName_).lastRestock = lastTime_;

        players[player_.guid].race.getUnitSpawnMap(player_.guid)[creepName_].lastRestock = lastTime_;
    }

}
