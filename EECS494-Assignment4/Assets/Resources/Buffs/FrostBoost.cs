using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class FrostBoost : Buff
{
    private static float SLOW_FACTOR;

    public override void Awake()
    {
        base.Awake();
        Init(1);
        name = "Damage Boost";
    }

    public override void Init(int level_)
    {
        FrostBoost fb = gameObject.GetComponent<FrostBoost>();
        if(fb != null)
        {
            level_ += fb.getLevel();
            float otherBuffDuration = (fb.birthTime + fb.duration) - GameState.getInstance().getGameTime();
            duration = fb.getLevel() * otherBuffDuration + level_ * 5.0f;
            Destroy(fb);
        }
        SLOW_FACTOR =  1 - Mathf.Pow(0.90f, level_);
        description = "+" + SLOW_FACTOR + " % slow";
        base.Init(level_);
    }

    public override void onApplication()
    {
        base.onApplication();
    }

    public override void onProjectile(Projectile p)
    {
        base.onProjectile(p);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
