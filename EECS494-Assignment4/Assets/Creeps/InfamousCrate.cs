using UnityEngine;
using System.Collections;

public class InfamousCrate : Creep 
{
    public override void Init(string ownerGUID)
    {
		prefab = Resources.Load ("Creeps/InfamousCreate") as GameObject;
        Init("Infamous Crate", ownerGUID, 100, 10, 10, 1);
    }
}
