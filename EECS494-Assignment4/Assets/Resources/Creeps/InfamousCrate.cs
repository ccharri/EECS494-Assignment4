using UnityEngine;
using System.Collections;

public class InfamousCrate : Creep 
{
    void Awake()
    {
        setName("Infamous Crate");
        setId("InfamousCrate");
        setHealth(100);
        setSpeed(10);
        setMana(10);
        setBounty(1);
        setLifeCost(1);
		cost = 5;
    }

	public void LoadPrefabs()
	{
		id = "infamousCrate";
		name = "Infamous Crate";
		cost = 5;
		bounty = 1;
		prefab = Resources.Load ("Creeps/InfamousCrate") as GameObject;
		if(prefab != null)
			Debug.Log ("Loaded InfamousCrate prefab");
		else
			Debug.Log ("InfamousCrate = null");
	}
}
