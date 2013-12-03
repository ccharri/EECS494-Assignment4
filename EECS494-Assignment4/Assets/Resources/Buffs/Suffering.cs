using UnityEngine;
using System.Collections;

public class Suffering : Buff<Creep> 
{
	private float DPS = 0 ;
	private float MULTPERPERCMISSING = 0;
	
	public Suffering(Creep target_, float duration_, float dps_, float mult_) : base(target_) 
	{
		duration = duration_;
		DPS = dps_;
		MULTPERPERCMISSING = mult_;
	}

	public Suffering(Creep target_, Unit owner_, float duration_, float dps_, float mult_) : base(target_, owner_) 
	{
		duration = duration_;
		DPS = dps_;
		MULTPERPERCMISSING = mult_;
	}
	
	public override void onApplication() 
	{
	}
	
	public override void onRemoval() {}
	
	public override bool FixedUpdate()
	{
		if (!target.isAlive())
			if (owner != null)
		{
			float missMult = MULTPERPERCMISSING * (1f - (target.getHealth()/target.getHealthMax()) * 100f);
			float damage = (DPS*(1f + missMult)) * Time.fixedDeltaTime;
			target.onDamage(damage);
		}
		return base.FixedUpdate();
	}
}