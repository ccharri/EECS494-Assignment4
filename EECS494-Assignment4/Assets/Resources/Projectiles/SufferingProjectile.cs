using UnityEngine;
using System.Collections;

public class SufferingProjectile : Projectile 
{
    public int levelOfSuffering;
    GameObject sufferingEffect;

    public virtual void applyBuff(Creep c) 
    {
        Suffering buff = c.gameObject.AddComponent<Suffering>();
        buff.Init(levelOfSuffering);
        buff.onApplication();
        buff.effect = Instantiate(sufferingEffect, buff.gameObject.transform.position, new Quaternion()) as GameObject;
    }
}
