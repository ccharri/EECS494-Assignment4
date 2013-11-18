using UnityEngine;
using System.Collections;

public class MagicMissile : Projectile
{
    public void Init(Creep target_)
    {
        base.Init(target_, 500);        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        home();
    }

    public override void OnCollisionEnter(Collision c)
    {
        print("Collided");
        if(c.gameObject == target)
        {
            target.onDamage(10.0);
            Destroy(this);
        }
    }
}
