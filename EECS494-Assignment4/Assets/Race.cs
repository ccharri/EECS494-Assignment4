using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Race{
	//Dictionary mapping unit internal name to the Spawnable base class.  
	Dictionary<string, Spawnable> towerMap = new Dictionary<string, Spawnable>();
	Dictionary<string, Spawnable> creepMap = new Dictionary<string, Spawnable>();

	public Spawnable getTower(string tower)
	{
		return towerMap[tower];
	}

	public Spawnable getCreep(string creep)
	{
		return creepMap[creep];
	}
}
