using UnityEngine;
using System.Collections.Generic;

public abstract class TargetingBehavior 
{
    //NOTE: All children of TargetingBehavior should define a "static T getInstance()"

    public abstract bool compare(Creep a, Creep b, Tower c);
    //DOES: Returns true if a is better than b. > operator
}
