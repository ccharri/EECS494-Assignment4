using UnityEngine;
using System.Collections.Generic;

public abstract class Projectile : Unit 
{
    protected Attribute speed;
    
    protected Tower owner;

    protected Creep target;
    protected Vector3 targetPos;
    
    protected double birthTime;

    public void Init(Creep target_, Tower owner_, double speed_)
    {
        speed = new Attribute(speed_);
        target = target_;
        owner = owner_;
        targetPos = target.transform.position;
        birthTime = Time.time;
    }

    public void replace(Projectile p_)
    {
        transform.position = p_.transform.position;
        rigidbody.velocity = p_.rigidbody.velocity;
    }

    protected Vector3 calculateHome()
    //DOES: Makes the projectile home on the target. Changes targetPos.
    {
        if(target == null)
            return new Vector3();
        targetPos = target.transform.position;
        Vector3 newVel = (targetPos - transform.position);
        newVel.Normalize();
        float scaleFactor = (float)speed.get() * Time.deltaTime;
        newVel.Scale(new Vector3(scaleFactor, scaleFactor, scaleFactor));
        return newVel;
    }


    public abstract void OnTriggerEnter(Collider collision);


	public float getSpeed()
	{
		return (float)speed.get();
	}

	public override void Update() 
    {
        base.Update();
	}

    public override void FixedUpdate()
    {
        if(Network.isServer)
        {
            base.FixedUpdate();
            if(target == null)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
