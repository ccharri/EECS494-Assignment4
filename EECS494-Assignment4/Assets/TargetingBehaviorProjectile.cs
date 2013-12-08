using UnityEngine;
using System.Collections.Generic;

interface TargetingBehaviorProjectile
{
    //NOTE: All children of TargetingBehavior should define a "static T getInstance()"

    public abstract bool compare(Projectile a, Projectile b, Tower c);
    //DOES: Returns true if a is better than b. > operator
}
