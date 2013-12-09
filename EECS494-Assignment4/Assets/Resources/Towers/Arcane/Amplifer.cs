using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Amplifer : Tower 
{
    //External Editor Attributess
    public string buffApplied = "Damage";
    public int buffAppliedLevel = 1;

    //Internal Book-Keeping
    protected TargetingBehaviorProjectile behavior = Closest.getInstance();

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual List<Projectile> getAllProjectilesInRadius(Vector3 origin, float radius)
    {
        List<Projectile> objects = new List<Projectile>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(origin.x, origin.z), radius);
        foreach(Collider2D c in colliders)
        {
            GameObject o = c.attachedRigidbody.gameObject;
            Projectile p = o.GetComponent<Projectile>();
            if(p != null && p.getOwner() == getOwner())
                objects.Add(p);
        }
        return objects;
    }

    protected override void FixedUpdate()
    {
        GameState g = GameState.getInstance();
        if(Network.isServer)
        {
            base.FixedUpdate();
            // Cooldown elapsed, Fire!
            if((lastFired + cooldown.get()) <= g.getGameTime())
                fire();
        }
    }

    protected virtual void fire()
    {
        Projectile p = findTarget();
        Buff b = p.gameObject.AddComponent(buffApplied) as Buff;
        b.Init(buffAppliedLevel);
        b.onApplication();
        //b.effect = Instantiate(sufferingEffect, buff.gameObject.transform.position, new Quaternion()) as GameObject;
        lastFired = GameState.getInstance().getGameTime();
    }

    protected virtual Projectile findTarget()
    {
        List<Projectile> projectiles = getAllProjectilesInRadius(gameObject.transform.position, range.get());
        if(projectiles.Count == 0)
            return null;
        Projectile newTarget = null;
        foreach(Projectile p in projectiles)
        {
            if(newTarget == null)
                newTarget = p;
            else if(behavior.compare(p, newTarget, this))
                newTarget = p;
        }
        return newTarget;
    }

    public override string getDescription()
    {
        return "Name: " + name + "\nBuff:" + buffApplied + "\nRange: " + rangeBase + "\nCooldown: " + cooldownBase;
    }
}
