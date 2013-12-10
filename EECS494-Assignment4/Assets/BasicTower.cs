using UnityEngine;
using System.Collections.Generic;

public class BasicTower : Tower 
{
    //External Unity Settings
    public Vector3 firePoint = new Vector3(0, 1, 0);
    public GameObject toFire;

    //Internal Book-Keeping
	protected List<Projectile> projectiles;
	protected Creep target;
	protected TargetingBehavior behavior = Closest.getInstance();

    protected override void Awake()
    {
        base.Awake();
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
        p.setOwner(getOwner());
        p.networkView.RPC("setTargetRPC", RPCMode.Others, target.networkView.viewID);
        p.setOwningTower(this);

        Buff[] buffs = GetComponents<Buff>();
        foreach(Buff b in buffs) 
            b.onProjectile(p);
  
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
                else if(behavior.compare(c, newTarget, this) < 0)
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

    public override string getDescription()
    {
        return "Name: " + name + "\nDamage: " + toFire.GetComponent<Projectile>().damageBase + "\nRange: " + rangeBase + "\nCooldown: " + cooldownBase;
    }
}
