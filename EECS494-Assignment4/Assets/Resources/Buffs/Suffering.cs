using UnityEngine;
using System.Collections;

public class Suffering : Buff
{
    //DOES: For each % of health missing, increase damage done by MISSING_HEALTH_MULTIPLIER.
	private float DPS = 0 ;
	private float MISSING_HEALTH_MULTIPLIER = 0;
    
    public override void Awake()
    {
        base.Awake();
        Init(1);
    }

    public override void Init(int level_)
    {
        duration = 1 * level_;
        DPS = level * 1.5f;
        MISSING_HEALTH_MULTIPLIER = level * 0.1f + 0.20f;
        base.Init(level_);
    }

    public override void onUpdate()
    {
        Creep c = GetComponent<Creep>();
        if(c == null)
            return;
        if(c.isAlive())
        {
            float missMult = MISSING_HEALTH_MULTIPLIER * ((1f - (c.getHealth() / c.getHealthMax())));
            float damage = (DPS * (1f + missMult)) * Time.fixedDeltaTime;
            c.onDamage(damage);
        }
        base.onUpdate();
    }
}