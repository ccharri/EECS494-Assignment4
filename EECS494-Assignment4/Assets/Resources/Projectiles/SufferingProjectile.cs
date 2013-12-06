using UnityEngine;
using System.Collections;

public class SufferingProjectile : Projectile 
{
    public int levelOfSuffering;

    public virtual void applyBuff(Creep c) 
    {
        Suffering buff = c.gameObject.AddComponent<Suffering>();
        buff.Init(levelOfSuffering);
        buff.onApplication();
    }	
}
