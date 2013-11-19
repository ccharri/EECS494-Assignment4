using UnityEngine;
using System.Collections;

public class InfamousCrate : Creep 
{
    public void Init()
    {
		prefab = Resources.Load ("Creeps/InfamousCreate") as GameObject;
        Init("Infamous Crate", Network.player.guid, 100, 10, 10, 1);
    }
}
