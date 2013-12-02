﻿using UnityEngine;
using System.Collections;

public class MagicMissile : Projectile
{
    public void Init(Creep target_, Tower owner_)
    {
        setTarget(target_);
        setOwner(owner_);
        setSpeed(7.50f);
        setDamage(10.0f);
    }

    public void Init(Creep target_, Tower owner_, float damage_, float speed_)
    {
        setTarget(target_);
        setOwner(owner_);
        setSpeed(speed_);
        setDamage(damage_);
    }

    protected override void FixedUpdate()
    {
        if(Network.isServer)
        {
            base.FixedUpdate();
            transform.rigidbody.transform.position += calculateHome();
        }
    }

    public override void OnTriggerEnter(Collider c)
    {
        if(Network.isServer)
        {
			if( target != null && c.gameObject.GetComponent<Creep>() == target)
            {
                target.onDamage(getDamage());
                destroy();
            }
        }
    }
}
