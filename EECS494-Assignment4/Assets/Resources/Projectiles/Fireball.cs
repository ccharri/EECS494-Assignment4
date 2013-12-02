using UnityEngine;
using System.Collections;

public class Fireball : Projectile
{
    float damage = 10;
    public void Init(Creep target_, Tower owner_)
    {
        setTarget(target_);
        setOwner(owner_);
        setSpeed(7.50f);
        damage = 10;
    }

    public void Init(Creep target_, Tower owner_, float damage_, float speed_)
    {
        setTarget(target_);
        setOwner(owner_);
        setSpeed(speed_);
        damage = damage_;
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
            if(target != null && c.gameObject.GetComponent<Creep>() == target)
            {
                target.onDamage(damage);
                destroy();
            }
        }
    }
}
