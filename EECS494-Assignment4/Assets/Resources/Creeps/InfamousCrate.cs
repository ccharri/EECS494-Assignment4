using UnityEngine;
using System.Collections;

public class InfamousCrate : Creep 
{
	public void LoadPrefabs()
	{
		id = "infamousCrate";
		name = "Infamous Crate";
		prefab = Resources.Load ("Creeps/InfamousCrate") as GameObject;
		if(prefab != null)
			Debug.Log ("Loaded InfamousCrate prefab");
		else
			Debug.Log ("InfamousCrate = null");
	}

    public override void Init(string ownerGUID)
    {
        Init("Infamous Crate", ownerGUID, 100, 10, 10, 1);
    }
}
