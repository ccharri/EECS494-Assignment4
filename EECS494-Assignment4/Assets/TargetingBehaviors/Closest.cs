using UnityEngine;
using System.Collections;

public class Closest : TargetingBehavior 
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

    private Closest() { }
}
