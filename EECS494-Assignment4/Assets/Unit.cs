using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour 
{
	public string name; //This is an internal name used for mapping and RPC calls
	protected string ownerID;
    protected List<Buff<Unit>> buffs;
    //TODO: Consider changing this to a map from BuffTag (string) to Buff for faster lookup

	public string getOwnerID() {return ownerID;}

    public void Start()
    {
        buffs = new List<Buff<Unit>>();
    }

    public void Init(string name_, string ownerID_)
    //NOTE: Init is only used when parameters need to be passed in.
    {
        name = name_;
        ownerID = ownerID_;
    }

    public virtual bool addBuff(Buff<Unit> b)
    {
        buffs.Add(b);
        return true;
    }

	public virtual bool removeBuff(Buff<Unit> b)
	{
		if(buffs.Remove(b))
		{
			b.onRemoval();
			return true;
		}
		return false;
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
