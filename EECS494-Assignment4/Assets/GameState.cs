using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState
{
	Dictionary<int, List<Creep>> creepsByArena;
    //NOTE: Returns the creeps in a player's arena. Indexed by pid

	Dictionary<int, List<Tower>> towersByPlayer;
    //NOTE: Returns towers owned by a player. Indexed by pid.

	Dictionary<int, PlayerState> players;

    public static GameState getGameState()
    {
        if(instance == null)
            instance = new GameState();
        return instance;
    }

    public void spawnCreepForPlayer(int pid, Creep c) //TODO: fix params? make it a prefab? change the method name?
    {
        if(creepsByArena.ContainsKey(pid))
            creepsByArena[pid].Add(c);
    }
    public void spawnTowerForPlayer(int pid, Tower t) //TODO: fix params? make it a prefab? change the method name?
    {
        if(towersByPlayer.ContainsKey(pid))
            towersByPlayer[pid].Add(t);
    }

    public List<Creep> getEnemyCreeps(int pid)
    {
        if(creepsByArena.ContainsKey(pid))
            return creepsByArena[pid];
        return null; //TODO: Throw an exception?
    }

    public void addPlayer(int pid)
    {
        creepsByArena.Add(pid, new List<Creep>());
        towersByPlayer.Add(pid, new List<Tower>());
        players.Add(pid, new PlayerState());
    }

    private static GameState instance;
    private GameState() 
    {
        creepsByArena = new Dictionary<int, List<Creep>>();
        towersByPlayer = new Dictionary<int, List<Tower>>();
        players = new Dictionary<int, PlayerState>();
    }
}
