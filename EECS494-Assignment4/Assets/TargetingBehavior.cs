using UnityEngine;
using System.Collections.Generic;

public interface TargetingBehavior 
{
    //NOTE: All children of TargetingBehavior should define a "static T getInstance()"

    bool compare(Creep a, Creep b, Tower c);
    //DOES: Returns true if a is better than b. > operator
}
