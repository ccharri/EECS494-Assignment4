using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour 
{
	public string name; //This is an internal name used for mapping and RPC calls
	public string id;
	protected string ownerGUID;

    public string getOwnerID() { return ownerGUID; }

    public void setName(string name_)   { name = name_; }
    public void setOwner(string guid_)  { ownerGUID = guid_;    }
    public void setId(string id_)       { id = id_; }

    public string getName()     { return name; }
    public string getOwner()    { return ownerGUID; }
    public string getId()       { return id; }

	protected virtual void Update() 
	{
		
	}
	protected virtual void FixedUpdate() 
	{

	}
}
