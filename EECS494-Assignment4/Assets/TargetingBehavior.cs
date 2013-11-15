using UnityEngine;
using System.Collections.Generic;

public abstract class TargetingBehavior 
{
    public static abstract TargetingBehavior getInstance();
    //DOES: Grabs an instance (global or new)
    public abstract bool compare(Creep a, Creep b, Tower c);
    //DOES: Returns true if a is better than b. > operator
}
