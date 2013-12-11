using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class SplashBoost : Buff
{
    private static float SPLASH_BOOST;
    private static float DAMAGE_PENALTY_MULTIPLIER;
    private static float SPEED_BOOST;

    public override void Awake()
    {
        base.Awake();
        Init(1);
        name = "Splash Boost";
    }

    public override void Init(int level_)
    {
        duration = 5;
        SPLASH_BOOST = 1f * level_;
        DAMAGE_PENALTY_MULTIPLIER = 1.0f - Mathf.Pow(0.8f, level_);
        SPEED_BOOST = 2 * level_;
        description = "+" + SPLASH_BOOST + " splash, -" + DAMAGE_PENALTY_MULTIPLIER + "% damage";
        base.Init(level_);
    }

    public override void onApplication()
    {
        base.onApplication();
    }

    public override void onProjectile(Projectile p)
    {
        p.addSplash(SPLASH_BOOST);
        p.addDamageFactor(-DAMAGE_PENALTY_MULTIPLIER);
        if(p.gameObject.particleSystem != null)
            p.gameObject.particleSystem.emissionRate += SPEED_BOOST;
        base.onProjectile(p);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
