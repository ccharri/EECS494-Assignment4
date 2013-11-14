using UnityEngine;
using System.Collections.Generic;

public abstract class TargetingBehavior 
{
    public abstract Creep findTarget(List<Creep> targets);
    //DOES: Picks the best target
    public abstract bool compare(Creep a, Creep b);
    //DOES: Returns true if a is better than b. > operator
}
