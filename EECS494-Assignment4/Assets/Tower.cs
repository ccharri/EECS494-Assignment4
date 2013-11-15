using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : Spawnable, Selectable 
{
	protected List<Projectile> projectiles;
	protected Creep target;
	protected TargetingBehavior behavior;
	
	protected Attribute range;
	protected Attribute cooldown;
	protected double lastFired;

    public Tower(double range_, double cooldown_)
    {
        range = new Attribute(range_);
        cooldown = new Attribute(cooldown_);
        behavior = Closest.getInstance();
    }

	public override void Update () 
	{
		base.Update();
	}
	public override void FixedUpdate() 
	{
		base.FixedUpdate();
        // Cooldown elapsed, Fire!
		if(lastFired + cooldown.get() > Time.time)
		{
            if(target == null)
                target = findTarget();
            if(target != null)
                fire();
            //OPT: Increment lastFired by a deltaTime*3~ to make this faster
		}
	}
	
	public virtual void fire()
	{
		lastFired = Time.time;
	}
	
	public virtual Creep findTarget()
	{
        List<Creep> arenaCreeps = GameState.getGameState().getEnemyCreeps(ownerId);
        Creep target = null;
        foreach(Creep c in arenaCreeps)
        {
            if(canFire(c))
            {
                if(target == null)
                    target = c;
                else if(behavior.compare(c, target, this))
                    target = c;
            }
        }
        return target;
	}

	public virtual bool canFire(Creep c)
	{
        if((c.transform.position - transform.position).magnitude > range.get())
            return false;
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
