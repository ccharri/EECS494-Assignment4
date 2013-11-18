using UnityEngine;
using System.Collections;

public class MagicMissile : Projectile
{
    public void Init(Creep target_)
    {
        base.Init(target_, 250);        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        transform.rigidbody.velocity = calculateHome();
    }

    public override void OnTriggerEnter(Collider c)
    {
        print("SADNESS");
        if(c.gameObject.GetComponent<Creep>() == target)
        {
            print("VICTORY");
            target.onDamage(10.0);
            Destroy(this.gameObject);
        }
    }
}
