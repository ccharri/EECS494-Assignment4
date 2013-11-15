﻿using UnityEngine;
using System.Collections;

public class MagicMissile : Projectile
{
    public MagicMissile(Creep target_) : base(target_)
    {

    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        home();
    }

    public override void OnCollisionEnter(Collision c)
    {
        if(c.gameObject == target)
        {
            target.onDamage(10.0);
            Destroy(this);
        }
    }
}
