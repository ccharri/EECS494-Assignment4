using UnityEngine;
using System.Collections;

public abstract class Spawnable : Unit 
{
    public int cost;
	public GameObject prefab;

	void protected override void FixedUpdate ()
	{
		base.FixedUpdate ();
	}
}
