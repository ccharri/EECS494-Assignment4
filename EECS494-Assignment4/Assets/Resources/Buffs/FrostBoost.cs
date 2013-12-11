using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class FrostBoost : Buff
{
    public override void Awake()
    {
        base.Awake();
        Init(1);
        name = "Frost Boost";
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
