using UnityEngine;
using System.Collections;

public class UnitSpawn 
{
	UnitSpawn(SpawnerInfo info)
	{
		initialStockTime = info.initialTimer;
		restockTime = info.restockTimer;
		maxStock = info.maxStock;
		currentStock = 0;
		spawn = info.gameObject.GetComponent<Unit>();
	}

	public Spawnable spawn;
	public float initialStockTime;
	public float restockTime;

	//Needs to be sent to player
	public float lastRestock;
	public int maxStock;

	//Needs to be sent to player
	public int currentStock;
}
