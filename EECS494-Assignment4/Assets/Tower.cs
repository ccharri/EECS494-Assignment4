using UnityEngine;
using System.Collections.Generic;

public class Tower : Spawnable, Selectable 
{
    //External Editor Attributess
    public float rangeBase = 10;
    public float cooldownBase = 1;

    public Vector3 firePoint = new Vector3(0, 1, 0);
    public string description = "";
    public GameObject toFire;

    //Internal Attribues
    protected Attribute range = new Attribute(1);
    protected Attribute cooldown = new Attribute(1);

    //Internal Book-Keeping
	protected List<Projectile> projectiles;
	protected Creep target;
	protected TargetingBehavior behavior = Closest.getInstance();
	protected double lastFired = 0;
    
    public void setRange(float range_)          { range.setBase(range_); }
    public void setCooldown(float cooldown_)    { cooldown.setBase(cooldown_); }

    public float getRange()                     { return range.get(); }
    public float getCooldown()                  { return cooldown.get(); }

    void Awake()
    {
        setRange(rangeBase);
        setCooldown(cooldownBase);
    }

	protected override void Update () 
	{
		base.Update();
	}
	
	protected override void FixedUpdate() 
	{
		GameState g = GameState.getInstance();
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
        GameObject proj = Network.Instantiate(toFire, transform.position + firePoint, transform.rotation, 0) as GameObject;
        Projectile p = proj.GetComponent<Projectile>();
        p.setTarget(target);
        p.setOwner(this);
		lastFired = GameState.getInstance().getGameTime();
	}
	
	protected virtual Creep findTarget()
	{
        List<Creep> arenaCreeps = GameState.getInstance().getEnemyCreeps(ownerGUID);
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

    public virtual string getDescription()
    {
        return description;
    }
	public virtual void mouseOverOn()
	{

	}
	public virtual void mouseOverOff()
	{

	}	
}
