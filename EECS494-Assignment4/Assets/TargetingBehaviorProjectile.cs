using UnityEngine;
using System.Collections.Generic;

public interface TargetingBehaviorProjectile
{
    //NOTE: All children of TargetingBehavior should define a "static T getInstance()"

    bool compare(Projectile a, Projectile b, Amplifer c);
    //DOES: Returns true if a is better than b. > operator
}
