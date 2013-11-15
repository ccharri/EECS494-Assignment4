using UnityEngine;
using System.Collections;

public class PlayerState{

	public PlayerState(string pid)
	{
		pid = pid;
		gold = 0;
		income = 0;
		lives = 10;
	}

	public string pid;
	public int gold;
	public int income;
	public int lives;
	public Race race;
}
