using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class SplitBoost : Buff
{
    private static int SPLITS;

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
//        SplitBoost sb = gameObject.GetComponent<SplitBoost>();
//        if(sb != null)
//        {
//            SPLITS = level_ + sb.getLevel();
//            level_ += sb.getLevel();
//            float otherBuffDuration = (sb.birthTime + sb.duration) - GameState.getInstance().getGameTime();
//            duration = sb.getLevel() * otherBuffDuration + level_ * 5.0f;
//            Destroy(sb);
//        }
//        else
            duration = 5.0f;
        description = "+" + SPLITS + " projectiles";
        base.Init(level_);
    }

    public override void onApplication()
    {
        base.onApplication();
    }

    public override void onProjectile(Projectile p)
    {
        base.onProjectile(p);

        spawned++;
        if(spawned == 1)
        {
            oldMult = p.getOwningTower().cooldown.getMultiplier();
            p.getOwningTower().cooldown.setMultiplier(0.2f);
        }
        if(spawned >= SPLITS)
            p.getOwningTower().cooldown.setMultiplier(oldMult);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
}
