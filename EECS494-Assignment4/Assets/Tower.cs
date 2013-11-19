using UnityEngine;
using System.Collections.Generic;

public abstract class Tower : Spawnable, Selectable 
{
    protected GameState g;

	protected List<Projectile> projectiles;
	protected Creep target;
	protected TargetingBehavior behavior = Closest.getInstance();
	
	protected Attribute range = new Attribute(1000);
	protected Attribute cooldown = new Attribute(1);
	protected double lastFired = 0;

    void Start()
    {
        g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
    }

    protected void Init(string name, string guid, float range_, float cooldown_)
    {
        range = new Attribute(range_);
        cooldown = new Attribute(cooldown_);
        Init(name, guid);
        g.addTowerForPlayer(guid, this);
    }

	protected override void Update () 
	{
		base.Update();
	}
	protected override void FixedUpdate() 
	{
        if(Network.isServer)
        {
            base.FixedUpdate();
            // Cooldown elapsed, Fire!
            if((lastFired + cooldown.get()) <= g.getGameTime())
            {

                if(target == null)
                    target = findTarget();
                if(target != null)
                    fire();
                //OPT: Increment lastFired by a deltaTime*3~ to make this faster
            }
        }
	}
	
	protected virtual void fire()
	{
        print("LAUNCHED");
		lastFired = g.getGameTime();
	}
	
	protected virtual Creep findTarget()
	{
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
