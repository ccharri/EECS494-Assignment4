﻿using UnityEngine;
using System.Collections;

public class PlayerState{
	public PlayerState(NetworkPlayer player_, Race race_)
	{
		player = player_;
		gold = 25;
		income = 10;
		lives = 10;
		race = race_;
   		race.addUnitSpawnMap(player_.guid);
		creepUpgradeLevel = 0;
	}

	public NetworkPlayer player;
	public int gold;
	public int income;
	public int lives;
	public Race race;

	public int creepUpgradeLevel;
}
