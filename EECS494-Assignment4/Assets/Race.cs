using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Race{
	//Dictionary Workaround
	//
	public List<string> towerMapKey = new List<string>();
	public List<Tower> towerMapValue = new List<Tower>();

	public List<string> creepMapKey = new List<string>();
	public List<Creep> creepMapValue = new List<Creep>();
	//


	//Dictionary mapping unit internal name to the Spawnable base class.  
	public Dictionary<string, Tower> towerMap = new Dictionary<string, Tower>();
	public Dictionary<string, Creep> creepMap = new Dictionary<string, Creep>();

    public Race()
    {
		//Zip up lists into Dictionary
		//
		//TowerMap
		for(int i = 0; i < towerMapKey.Count; i++)
		{
			towerMap.Add(towerMapKey[0], towerMapValue[0]);
		}
		//CreepMap
		for(int i = 0; i < creepMapKey.Count; i++)
		{
			creepMap.Add(creepMapKey[0], creepMapValue[0]);
		}
		//

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
