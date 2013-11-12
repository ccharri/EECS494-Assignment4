using UnityEngine;
using System.Collections;

public class OnFire : Buff<Creep> 
{
    private static double DURATION = 10;
    private static double BURN_DAMAGE = 10;

    public OnFire(Creep target_) : base(target_) {}
    public OnFire(Creep target_, Unit owner_) : base(target_, owner_) {}

    public override void onApplication() 
    {
        duration = DURATION;
    }

    public override void onRemoval() {}

    public override bool FixedUpdate()
    {
        target.onDamage(BURN_DAMAGE);
        if (!target.isAlive())
            if (owner != null)
            {
                //TODO: Add bounty? Claim
            }
        return base.FixedUpdate();
    }
}