using UnityEngine;
using System.Collections;

public class UnitSpawn 
{
	public Spawnable spawn;
	public float initialStockTime;
	public float restockTime;

	//Needs to be sent to player
	public float lastRestock;
	public int maxStock;

	//Needs to be sent to player
	public int currentStock;
}
