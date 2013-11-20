using UnityEngine;
using System.Collections;

public class MagicMissile : Projectile
{

    public void Init(Creep target_, Tower owner_)
    {
        setTarget(target_);
        setOwner(owner_);
        setSpeed(750);
    }

    protected override void FixedUpdate()
    {
        if(Network.isServer)
        {
            base.FixedUpdate();
            transform.rigidbody.velocity = calculateHome();
        }
    }

    public override void OnTriggerEnter(Collider c)
    {
        if(Network.isServer)
        {
            if(c.gameObject.GetComponent<Creep>() == target && target != null)
            {
                target.onDamage(10.0f);
                Destroy(this.gameObject);
            }
        }
    }
}
