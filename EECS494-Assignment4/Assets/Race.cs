using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Race{
	//Dictionary mapping unit internal name to the Spawnable base class.  
	public Dictionary<string, Tower> towerMap = new Dictionary<string, Tower>();
	public Dictionary<string, Creep> creepMap = new Dictionary<string, Creep>();

    public Race()
    {
		ArcaneTower atower = new ArcaneTower();
		atower.LoadPrefabs();

		InfamousCrate icrate = new InfamousCrate();
		icrate.LoadPrefabs();

        towerMap.Add(atower.id, atower);
        creepMap.Add(icrate.id, icrate);
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
