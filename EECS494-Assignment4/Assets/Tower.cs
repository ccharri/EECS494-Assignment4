using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : Spawnable, Selectable 
{
	List<Projectile> projectiles;
	Creep target;
	TargetingBehavior behavior;
	
	Attribute range;
	Attribute cooldown;
	double lastFired; 

	public override void Update () 
	{
		base.Update();
	}
	public override void FixedUpdate() 
	{
		base.FixedUpdate();
		//cooldown elapsed
		if(lastFired + cooldown.get() > Time.time)
		{
			//find target
			fire();
		}
	}
	
	public void fire()
	{
		lastFired = Time.time;
	}

	
	/*public Creep findTarget()
	{

	}*/
	public bool canFire(Creep c)
	{
		//range check
		return true;
	}
	

	public abstract string getDescription();
	public void mouseOverOn()
	{
		//TODO: Implement
	}
	public void mouseOverOff()
	{
		//TODO: Implement
	}	
}
