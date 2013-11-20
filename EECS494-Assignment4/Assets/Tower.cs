using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : Spawnable, Selectable 
{
    protected GameState g;

	protected List<Projectile> projectiles;
	protected Creep target;
	protected TargetingBehavior behavior = Closest.getInstance();
	
	protected Attribute range = new Attribute(1);
	protected Attribute cooldown = new Attribute(1);
	protected double lastFired = 0;

    void Start()
    {
        g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
    }

    public void setRange(float range_)          { range = new Attribute(range_); }
    public void setCooldown(float cooldown_)    { cooldown = new Attribute(cooldown_); }

    public float getRange()                     { return range.get(); }
    public float getCooldown()                  { return cooldown.get(); }

	protected override void Update () 
	{
		base.Update();
	}
	protected override void FixedUpdate() 
	{
        g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
        if(Network.isServer)
        {
            base.FixedUpdate();
            // Cooldown elapsed, Fire!
            if((lastFired + cooldown.get()) <= g.getGameTime())
            {
                if(target == null || canFire(target) == false)
                    target = findTarget();
                if(target != null)
                    fire();
                //OPT: Increment lastFired by a deltaTime*3~ to make this faster
            }
        }
	}
	
	protected virtual void fire()
	{
		lastFired = g.getGameTime();
	}
	
	protected virtual Creep findTarget()
	{
        List<Creep> arenaCreeps = g.getEnemyCreeps(ownerGUID);
        if(arenaCreeps.Count == 0)
            return null;
        Creep newTarget = null;
        foreach(Creep c in arenaCreeps)
        {
            if(canFire(c))
            {
                if(newTarget == null)
                    newTarget = c;
                else if(behavior.compare(c, newTarget, this))
                    newTarget = c;
            }
        }
        return newTarget;
	}

	protected virtual bool canFire(Creep c)
	{
        if((c.transform.position - transform.position).magnitude > range.get())
            return false;
        return true;
	}
	
	public abstract string getDescription();
	public void mouseOverOn()
	{

	}
	public void mouseOverOff()
	{

	}	
}
