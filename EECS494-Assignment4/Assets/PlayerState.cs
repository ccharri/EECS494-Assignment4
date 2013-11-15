using UnityEngine;
using System.Collections;

public class PlayerState{

	public PlayerState(NetworkPlayer player_)
	{
		player = player_;
		gold = 0;
		income = 0;
		lives = 10;
	}

	public NetworkPlayer player;
	public int gold;
	public int income;
	public int lives;
	public Race race;
}
