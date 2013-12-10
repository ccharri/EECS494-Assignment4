using UnityEngine;
using System.Collections.Generic;

public interface TargetingBehaviorTower
{
    //NOTE: All children of TargetingBehavior should define a "static T getInstance()"

    double compare(Tower a, Tower b, Amplifer c);
    //DOES: Returns true if a is better than b. > operator
}
