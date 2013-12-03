﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Race {
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

  public Dictionary<string, UnitSpawn> unitSpawnMap = new Dictionary<string, UnitSpawn>();

	public void Zip()
	{		
		//Zip up lists into Dictionary
		//
		//TowerMap
		
		//
		for(int i = 0; i < towerMapKey.Count; i++)
		{
			Debug.Log ("Adding Tower {"+towerMapKey[i]+":"+towerMapValue[i]+"}");
			towerMap.Add(towerMapKey[i], towerMapValue[i]);
		}
		//CreepMap
		for(int i = 0; i < creepMapKey.Count; i++)
		{
			Debug.Log ("Adding Creep {"+creepMapKey[i]+":"+creepMapValue[i]+"}");
			creepMap.Add(creepMapKey[i], creepMapValue[i]);
		}
    
    //UnitSpawn
    for (int i = 0; i < creepMapKey.Count; i++)
    {
        Debug.Log("Adding Unitspawn for: " + creepMapKey[i]);
        unitSpawnMap.Add(creepMapKey[i], new UnitSpawn());
    }
	}

  void Awake()
	{
		Zip ();
	}

    public Race()
    {
		Zip();
//
//		ArcaneTower atower = new ArcaneTower();
//		atower.LoadPrefabs();
//
//		InfamousCrate icrate = new InfamousCrate();
//		icrate.LoadPrefabs();
//
//        towerMap.Add(atower.id, atower);
//        creepMap.Add(icrate.id, icrate);
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

  public UnitSpawn getUnitSpawn(string creep)
  {
      if (unitSpawnMap.ContainsKey(creep))
          return unitSpawnMap[creep];
      return null;
  }

	public override string ToString ()
	{
		string val = "{Towers:";
		foreach(string key in towerMap.Keys)
		{
			val += "{"+key+":"+towerMap[key] != null ? towerMap[key].ToString() : "null"+"}";
		}
		val += "},{Creeps:";
		foreach(string key in creepMap.Keys)
		{
			val += "{"+key+":"+creepMap[key]!= null ? creepMap[key].ToString() : "null"+"}";
		}
		val += "}";

		return val;
	}
}
