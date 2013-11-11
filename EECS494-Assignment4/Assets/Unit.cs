using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour 
{
    int ownerId;
    List<Buff> buffs;
    //TODO: Consider changing this to a map from BuffTag (string) to Buff for faster lookup

	public virtual void Update() 
	{
		//other things yay!
	}
	public virtual void FixedUpdate() 
	{
		
	}
}
