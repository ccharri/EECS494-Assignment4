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


    public Tower()
    {
        tag = "Tower";
    }

	public override void Update () 
	{
		base.Update();
	}
	public override void FixedUpdate() 
	{
		base.FixedUpdate();
        // Cooldown elapsed, fire
		if(lastFired + cooldown.get() > Time.time)
		{
            target = findTarget();
			fire();
		}
	}
	
	public virtual void fire()
	{
		lastFired = Time.time;
	}

	
	public Creep findTarget() //TODO: Make this?
	{
        //GameState.getGameState().
        return null;
	}
	public virtual bool canFire(Creep c)
	{
		//range check?
		return true;
	}
	

	public abstract string getDescription();

    Light originalLight;
	public void mouseOverOn()
	{
        //TODO: Finish highlighting code
        originalLight = GetComponent<Light>();
        Light newLight = new Light();
        newLight.intensity = 1000;
        

		//TODO: Implement
	}
	public void mouseOverOff()
	{
		//TODO: Implement
	}	
}
