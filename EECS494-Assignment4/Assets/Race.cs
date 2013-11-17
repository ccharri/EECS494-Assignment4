using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Race{
	//Dictionary mapping unit internal name to the Spawnable base class.  
	Dictionary<string, Tower> towerMap = new Dictionary<string, Tower>();
	Dictionary<string, Creep> creepMap = new Dictionary<string, Creep>();

	public Tower getTower(string tower)
	{
		return towerMap[tower];
	}

	public Creep getCreep(string creep)
	{
		return creepMap[creep];
	}
}
