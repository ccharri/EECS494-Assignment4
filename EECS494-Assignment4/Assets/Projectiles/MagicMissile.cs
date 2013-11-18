using UnityEngine;
using System.Collections;

public class MagicMissile : Projectile
{
    public void Init(Creep target_, Tower owner_)
    {
        base.Init(target_, owner_, 250);        
    }

    public override void FixedUpdate()
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
            print("SADNESS");
            if(c.gameObject.GetComponent<Creep>() == target && target != null)
            {
                print("VICTORY");
                target.onDamage(10.0);
                Destroy(this.gameObject);
            }
        }
    }
}
