using UnityEngine;
using System.Collections;

public class DamageBoost : Buff
{
    private static float DAMAGE_BOOST;
    private static float EMISSION_RATE_BOOST;

    public override void Awake()
    {
        base.Awake();
        Init(1);
    }

    public override void Init(int level_)
    {
        base.Init(level_);
        duration = 1000;
        DAMAGE_BOOST = 30 * level_;
        EMISSION_RATE_BOOST = 30 * level_;
    }

    public override void onApplication()
    {
        Projectile p = GetComponent<Projectile>();
        if(p == null)
            return;
        p.addDamage(DAMAGE_BOOST);
        gameObject.particleSystem.emissionRate += EMISSION_RATE_BOOST;
    }
    public override void OnDestroy()
    {
        Projectile p = GetComponent<Projectile>();
        if(p == null)
            return;
        p.addDamage(-DAMAGE_BOOST);
        gameObject.particleSystem.emissionRate -= EMISSION_RATE_BOOST;
        base.OnDestroy();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}