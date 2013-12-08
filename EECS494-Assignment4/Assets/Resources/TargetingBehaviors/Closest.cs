using UnityEngine;
using System.Collections;

public class Closest : TargetingBehavior, TargetingBehaviorProjectile 
{
    private static Closest instance;

    public static Closest getInstance()
    {
        if(instance == null)
            instance = new Closest();
        return instance;
    }
    public bool compare(Creep a, Creep b, Tower c)
    {
        double distA = (a.transform.position - c.transform.position).sqrMagnitude;
        double distB = (a.transform.position - c.transform.position).sqrMagnitude;
        return distA <= distB;
    }
    public bool compare(Projectile a, Projectile b, Tower c)
    {
        double distA = (a.transform.position - c.transform.position).sqrMagnitude;
        double distB = (a.transform.position - c.transform.position).sqrMagnitude;
        return distA <= distB;
    }

    private Closest() { }
}
