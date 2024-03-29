﻿using UnityEngine;
using System.Collections;

public class DamageBoost : Buff
{
    private static float DAMAGE_BOOST;
    private static float EMISSION_RATE_BOOST;

    public override void Awake()
    {
        base.Awake();
        Init(1);
        name = "Damage Boost";
    }

    public override void Init(int level_)
    {
        duration = 5;
        DAMAGE_BOOST = 15 * level_;
        EMISSION_RATE_BOOST = 2 * level_;
        description = "+" + DAMAGE_BOOST + " damage";
        base.Init(level_);
    }

    public override void onApplication()
    {
        if(gameObject.particleSystem != null)
            gameObject.particleSystem.emissionRate += EMISSION_RATE_BOOST;
        base.onApplication();
    }

    public override void onProjectile(Projectile p)
    {
        p.addDamage(DAMAGE_BOOST);
        if(p.gameObject.particleSystem != null)
            p.gameObject.particleSystem.emissionRate += EMISSION_RATE_BOOST;
        base.onProjectile(p);
    }

    public override void OnDestroy()
    {
        if(gameObject.particleSystem != null)
            gameObject.particleSystem.emissionRate -= EMISSION_RATE_BOOST;
        base.OnDestroy();
    }
}