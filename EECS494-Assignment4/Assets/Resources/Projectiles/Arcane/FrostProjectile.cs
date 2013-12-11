using UnityEngine;
using System.Collections;

public class FrostProjectile : Projectile
{
    public int levelOfFrost;
    public GameObject frostEffect;

    public override void applyBuff(Creep c)
    {
        Suffering buff = c.gameObject.AddComponent<Suffering>();
        buff.Init(levelOfFrost);
        buff.onApplication();
        buff.effect = Instantiate(frostEffect, buff.gameObject.transform.position, new Quaternion()) as GameObject;
    }
}
