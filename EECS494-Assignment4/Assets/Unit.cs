﻿using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour 
{
	public string name; //This is an internal name used for mapping and RPC calls
	public string id;
	protected string ownerGUID;
    protected List<Buff<Unit>> buffs = new List<Buff<Unit>>();
    //TODO: Consider changing this to a map from BuffTag (string) to Buff for faster lookup

    public string getOwnerID() { return ownerGUID; }

    public void setName(string name_)   { name = name_; }
    public void setOwner(string guid_)  { ownerGUID = guid_;    }
    public void setId(string id_)       { id = id_; }

    public string getName()     { return name; }
    public string getOwner()    { return ownerGUID; }
    public string getId()       { return id; }


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

	protected virtual void Update() 
	{
		
	}
	protected virtual void FixedUpdate() 
	{
		foreach(Buff<Unit> b in buffs)
            b.FixedUpdate();
	}
}
