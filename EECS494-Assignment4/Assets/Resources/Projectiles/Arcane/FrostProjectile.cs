using UnityEngine;
using System.Collections;

public class FrostProjectile : Projectile
{
    public int levelOfFrost;
    public GameObject frostEffect;

    public override void applyBuff(Creep c)
    {
        Slowed buff = c.gameObject.AddComponent<Slowed>();
        buff.Init(levelOfFrost);
        buff.onApplication();
    }
}
