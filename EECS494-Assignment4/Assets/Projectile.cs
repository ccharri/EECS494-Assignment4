using UnityEngine;
using System.Collections.Generic;

public abstract class Projectile : Unit 
{
    protected Attribute speed = new Attribute(1);
    protected Tower owner = null;
    protected Creep target = null;
    protected Vector3 targetPos = new Vector3(0,0,0);
    protected double birthTime = 0;

    public void setSpeed(float speed_)              { speed = new Attribute(speed_); }
    public void setTarget(Creep target_)            { target = target_; setTargetPos(target.transform.position); }
    public void setOwner(Tower owner_)              { owner = owner_; }
    public void setTargetPos(Vector3 targetPos_)    { targetPos = targetPos_; }

    public float getSpeed()         { return speed.get(); }
    public Creep getTarget()        { return target;}
    public Tower getOwner()         { return owner;}
    public Vector3 getTargetPos()   { return targetPos;}


    void Awake()
    {
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
        float scaleFactor = (float)speed.get() * Time.fixedDeltaTime;
		newVel *= scaleFactor;
        return newVel;
    }

    public void destroy()
    {
        Network.Destroy(this.gameObject);
    }

    public abstract void OnTriggerEnter(Collider collision);


	protected override void Update() 
    {
        base.Update();
	}

    protected override void FixedUpdate()
    {
        if(Network.isServer)
        {
            base.FixedUpdate();
            if(target == null)
            {
                Network.Destroy(this.gameObject);
            }
        }
    }
}
