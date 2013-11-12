using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState
{
	Dictionary<int, List<Creep>> creeps;
	Dictionary<int, List<Tower>> towers;
	Dictionary<int, PlayerState> players;

    public static GameState getGameState()
    {
        if (instance == null)
            instance = new GameState();
        return instance;
    }




    static GameState instance;
    private GameState() {}
}
