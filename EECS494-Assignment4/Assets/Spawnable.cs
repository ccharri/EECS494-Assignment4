using UnityEngine;
using System.Collections;

public abstract class Spawnable : Unit 
{
    public int cost;
	public GameObject prefab;

    public abstract void Init(string ownerGUID_);
    //DOES: Sets up the single piece of data that the unit needs after Instaniation.
}
