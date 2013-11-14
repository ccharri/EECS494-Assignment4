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
        if (instance == null)
            instance = new GameState();
        return instance;
    }

    void spawnCreepForPlayer(int pid, Creep c) //TODO: fix params? make it a prefab? change the method name?
    {
        if (creepsByArena.ContainsKey(pid))
            creepsByArena[pid].Add(c);
    }

    List<Creep> getEnemyCreeps(int pid)
    {
        if(creepsByArena.ContainsKey(pid))
            return creepsByArena[pid];
        return null; //TODO: Throw an exception?
    }


    static GameState instance;
    private GameState() {}
}
