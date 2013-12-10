using UnityEngine;
using System.Collections;

public class UnitSpawn 
{
	public UnitSpawn(SpawnerInfo info)
	{
		initialStockTime = info.initialTimer;
		restockTime = info.restockTimer;
		maxStock = info.maxStock;
		currentStock = 0;
		Creep creep = info.gameObject.GetComponent<Creep>();
		cost = creep.cost;
		income = creep.bounty;
		spawn = creep;
	}

	public Spawnable spawn;
	public float initialStockTime;
	public float restockTime;

	//Needs to be sent to player
	public float lastRestock;
	public int maxStock;

	//Needs to be sent to player
	public int currentStock;

	public int cost;
	public int income;
}
