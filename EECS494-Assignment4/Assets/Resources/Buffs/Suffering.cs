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
        base.Init(level_);
        duration = 5;
        DPS = level * 5;
        MISSING_HEALTH_MULTIPLIER = level * 0.25f + 0.25f;
    }

	public override void onApplication() 
    {
        base.onApplication();
    }
	public override void onRemoval() {}

    public override void FixedUpdate()
    {
        if(!enabled)
            return;
        Creep c = GetComponent<Creep>();
        if(c == null)
            return;
        if(c.isAlive())
        {
            float missMult = MISSING_HEALTH_MULTIPLIER * (1f - (c.getHealth() / c.getHealthMax()) * 100f);
            float damage = (DPS * (1f + missMult)) * Time.fixedDeltaTime;
            c.onDamage(damage);
        }
        base.FixedUpdate();
    }
}