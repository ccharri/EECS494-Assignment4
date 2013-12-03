using UnityEngine;
using System.Collections;

public class TestCreep : Creep 
{
    void Awake()
    {
        setName("Test Creep");
        setId("TestCreep");
        setHealth(100);
        setSpeed(250);
        setMana(10);
        setBounty(1);
        setLifeCost(1);
		cost = 5;
    }

	public void LoadPrefabs()
	{
		id = "testCreep";
		name = "Test Creep";
		cost = 5;
		bounty = 1;
		prefab = Resources.Load ("Creeps/TestCreep") as GameObject;
		if(prefab != null)
			Debug.Log ("Loaded TestCreep prefab");
		else
			Debug.Log ("TestCreep = null");
	}
}
