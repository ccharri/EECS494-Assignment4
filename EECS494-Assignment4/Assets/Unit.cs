using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour 
{
    protected int ownerId;
    protected List<Buff<Unit>> buffs;
    //TODO: Consider changing this to a map from BuffTag (string) to Buff for faster lookup


    public virtual bool addBuff(Buff<Unit> b)
    {
        buffs.Add(b);
        return true;
    }
	public virtual void Update() 
	{
		
	}
	public virtual void FixedUpdate() 
	{
		foreach(Buff<Unit> b in buffs)
            b.FixedUpdate();
	}
}
