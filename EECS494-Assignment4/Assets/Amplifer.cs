using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Amplifer : Tower 
{
    //External Editor Attributess
    public int buffAppliedLevel = 1;
    public static float standardCooldown = 5;

    //Internal Book-Keeping
    protected TargetingBehaviorTower behavior = Closest.getInstance();

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual Buff addBuff(Tower t) 
    {
        return null;
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
        Tower t = findTarget();
        if(t != null)
        {
            addBuff(t);
            lastFired = GameState.getInstance().getGameTime();
        }
    }

    protected Tower findTarget()
    {
        List<Tower> towers = GameState.getInstance().getPlayerTowers(getOwner());
        Tower newTarget = null;
        int sameCompareCount = 1;
        foreach(Tower t in towers)
        {
            if(canFire(t))
            {
                if(newTarget == null)
                    newTarget = t;
                else
                {
                    int compare = (int)behavior.compare(t, newTarget, this);
                    if(compare < 0)
                    {
                        newTarget = t;
                        sameCompareCount = 1;
                    }
                    else if(compare == 0)
                    {
                        sameCompareCount++;
                        if(Random.value * sameCompareCount < 1)
                            newTarget = t;
                    }
                }
            }
        }
        return newTarget;
    }

    protected virtual bool canFire(Tower t)
    {
        if((t.transform.position - transform.position).magnitude > range.get())
            return false;
        return true;
    }

    public override string getDescription()
    {
        return "Name: " + "\nRange: " + rangeBase + 
                "\nCooldown: " + cooldownBase +
                "\nEffect: Buffs the damage of adjacent towers";
    }
}
