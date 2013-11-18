using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : Spawnable, Selectable 
{
	protected List<Projectile> projectiles;
	protected Creep target;
	protected TargetingBehavior behavior = Closest.getInstance();
	
	protected Attribute range = new Attribute(1000);
	protected Attribute cooldown = new Attribute(1);
	protected double lastFired = 0;

    public void Init(double range_, double cooldown_)
    {
        range = new Attribute(range_);
        cooldown = new Attribute(cooldown_);
    }

	public override void Update () 
	{
		base.Update();
	}
	public override void FixedUpdate() 
	{
        GameState g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
		base.FixedUpdate();
        // Cooldown elapsed, Fire!
		if((lastFired + cooldown.get()) > g.getGameTime())
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
        GameState g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
        List<Creep> arenaCreeps = g.getEnemyCreeps(ownerGUID);
        if(arenaCreeps.Count == 0)
            return null;
        Creep target = arenaCreeps[0];
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
