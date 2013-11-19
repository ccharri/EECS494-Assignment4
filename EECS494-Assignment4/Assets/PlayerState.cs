using UnityEngine;
using System.Collections;

public class PlayerState{

	public PlayerState(NetworkPlayer player_)
	{
		player = player_;
		gold = 25;
		income = 25;
		lives = 10;

		race = new Race();
	}

	public NetworkPlayer player;
	public int gold;
	public int income;
	public int lives;
	public Race race;
}
