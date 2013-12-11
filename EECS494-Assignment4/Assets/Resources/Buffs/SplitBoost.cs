using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class SplitBoost : Buff
{
    private static int SPLITS;
    private static float DAMAGE_PENALTY_FACTOR;
    private static float SPLASH_PENALTY_FACTOR;

    private int spawned;
    private float oldMult;

    public override void Awake()
    {
        base.Awake();

        Init(1);
        name = "Damage Boost";
    }

    public override void Init(int level_)
    {
        duration = 5;
        SPLITS = level_;
        DAMAGE_PENALTY_FACTOR = 1.0f - (1.0f/level_ + 0.15f);
        SPLASH_PENALTY_FACTOR = 1.0f - (1.0f/level_ + 0.15f);
        description = "+" + SPLITS + " projectiles";
        base.Init(level_);
    }

    public override void onApplication()
    {
        base.onApplication();
    }

    public override void onProjectile(Projectile p)
    {
        p.addDamageFactor(-DAMAGE_PENALTY_FACTOR);
        p.addSplashFactor(-SPLASH_PENALTY_FACTOR);
        base.onProjectile(p);

        spawned++;
        if(spawned == 1)
        {
            oldMult = p.getOwningTower().cooldown.getMultiplier();
            p.getOwningTower().cooldown.setMultiplier(0);
        }
        if(spawned >= SPLITS)
            p.getOwningTower().cooldown.setMultiplier(oldMult);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
