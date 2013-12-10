using UnityEngine;
using System.Collections;

public class Closest : TargetingBehavior, TargetingBehaviorTower
{
    private static Closest instance;

    public static Closest getInstance()
    {
        if(instance == null)
            instance = new Closest();
        return instance;
    }
    public double compare(Creep a, Creep b, Tower c)
    {
        double distA = (a.transform.position - c.transform.position).sqrMagnitude;
        double distB = (a.transform.position - c.transform.position).sqrMagnitude;
        return distA - distB;
    }
    public double compare(Tower a, Tower b, Amplifer c)
    {
        double distA = (a.transform.position - c.transform.position).sqrMagnitude;
        double distB = (a.transform.position - c.transform.position).sqrMagnitude;
        return distA - distB;
    }

    private Closest() { }
}
