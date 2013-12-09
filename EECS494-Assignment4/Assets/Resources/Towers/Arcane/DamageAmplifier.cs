using UnityEngine;
using System.Collections;

public class DamageAmplifier : Amplifer 
{
    protected override Buff addBuff(Projectile p)
    {
        Buff b = p.gameObject.AddComponent<DamageBoost>();
        b.Init(buffAppliedLevel);
        b.onApplication();
        return b;
    }
}
