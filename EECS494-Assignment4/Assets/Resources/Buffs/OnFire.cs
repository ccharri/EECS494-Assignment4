using UnityEngine;
using System.Collections;

public class OnFire : Buff
{
    private static float BURN_DAMAGE;

    public override void Awake()
    {
        base.Awake();
        Init(1);
    }

    public override void Init(int level_)
    {
        base.Init(level_);
        duration = 10;
        BURN_DAMAGE = 10 * level_;
    }

    public override void onApplication()
    {
        OnFire oldBuff = GetComponent<OnFire>();
        if(oldBuff != null & oldBuff.getLevel() == getLevel())
        {
            oldBuff.setDuration(oldBuff.getDuration() + getDuration());
            Destroy(this);
        }
    }
    public override void onRemoval() {}

    public override void FixedUpdate()
    {
        if(!getEnabled())
            return;
        Creep c = GetComponent<Creep>();
        if(c == null)
            return;

        c.onDamage(BURN_DAMAGE);
        if (!c.isAlive())
            if (owner != null)
            {
                //TODO: Add bounty? Claim
            }
        base.FixedUpdate();
    }
}