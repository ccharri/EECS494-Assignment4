using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaceDatabase {
	private static Dictionary<NetworkPlayer, string> races = new Dictionary<NetworkPlayer, string>();
	
	public static void setRace(NetworkPlayer player, string race)
	{
		races[player] = race;
	}

	public static string getRace(NetworkPlayer player)
	{
		return races[player];
	}
}