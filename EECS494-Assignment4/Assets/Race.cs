using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Race{
	//Dictionary mapping unit internal name to the Spawnable base class.  
	public Dictionary<string, Tower> towerMap = new Dictionary<string, Tower>();
	public Dictionary<string, Creep> creepMap = new Dictionary<string, Creep>();

    public Race()
    {
        towerMap.Add("Arcane Tower", new ArcaneTower());
        creepMap.Add("Infamous Crate", new InfamousCrate());
    }

	public Tower getTower(string tower)
	{
		if (towerMap.ContainsKey(tower))
		{
			return towerMap[tower];
		}
		return null;
	}

	public Creep getCreep(string creep)
	{
		if (creepMap.ContainsKey(creep))
		{
			return creepMap[creep];
		}
		return null;
	}
}
