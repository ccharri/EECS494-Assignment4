using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Amplifer : Tower 
{
    //External Editor Attributess
    public int buffAppliedLevel = 1;

    //Internal Book-Keeping
    protected TargetingBehaviorProjectile behavior = Closest.getInstance();

    protected override void Awake()
    {
        base.Awake();
    }

    protected virtual Buff addBuff(Projectile p) 
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
        //get target tower
        //if(p == null)
           //return;
        //Debug.Log("LISTEN THOMAS, I HAVE SOMETHING: FUCK");
        //Buff b = addBuff(p);
        lastFired = GameState.getInstance().getGameTime();
    }

    protected Tower findTarget()
    {
        List<Tower> towers = GameState.getInstance().getPlayerTowers(getOwner());

        return towers[0];
        
    }

    public override string getDescription()
    {
        return "Name: " + "\nRange: " + rangeBase + "\nCooldown: " + cooldownBase;
    }
}
