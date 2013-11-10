using UnityEngine;
using System.Collections;

public class Tower : Spawnable, Selectable 
{
	Creep target;
	Attribute cooldown; 

	public override void Update () 
	{
		base.Update();
		//other things yay!
	}
	public override void FixedUpdate() 
	{
		base.FixedUpdate();

	}

	public string getDescription()
	{
		//TODO: Implement
		return "REPLACE ME?";
	}
	public void mouseOverOn()
	{
		//TODO: Implement

	}
	public void mouseOverOff()
	{
		//TODO: Implement

	}	
}
