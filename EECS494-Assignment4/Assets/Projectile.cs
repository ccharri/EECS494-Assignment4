using UnityEngine;
using System.Collections.Generic;

public class Projectile : Unit 
{
    //External Editor Initial Settings
    public float damageBase = 10;
    public float speedBase = 7.5f;
    public float splashBase = 0;

    public Buff<Creep> appliedBuff = null;
    public bool homing = true;

    //Internal Attributes
    protected Attribute speed = new Attribute(1);
    protected Attribute damage = new Attribute(1);
    protected Attribute splash = new Attribute(1);
    public Tower owningTower = null;
    public Creep target = null;
	public Transform targetTrans;
    public Vector3 targetPos = new Vector3(0,0,0);
    public double birthTime = 0;

    public void setSpeed(float speed_)              { speed.setBase(speed_); }
    public void setDamage(float damage_)            { damage.setBase(damage_); }
    public void setSplash(float splash_)            { splash.setBase(splash_); }
    public void setTarget(Creep target_)            
    { 
        target = target_;
        Vector3 pos = target.gameObject.transform.position;
		Debug.Log ("Target position = " + pos);
        setTargetPos(new Vector3(pos.x, pos.y, pos.z)); 
		targetTrans = target.gameObject.transform;
    }
    public void setOwningTower(Tower owner_)        { owningTower = owner_; }
	public void setTargetPos(Vector3 targetPos_)    { targetPos = targetPos_; Debug.Log ("Target position = " + targetPos_);}

    public float getSpeed()         { return speed.get(); }
    public float getSpeedBase()     { return speed.getBase(); }
    public float getDamage()        { return damage.get(); }
    public float getSplash()        { return splash.get(); }
    public Creep getTarget()        { return target; }
    public Tower getOwningTower()   { return owningTower; }
    public Vector3 getTargetPos()   { return targetPos; }


    void Awake()
    {
        birthTime = Time.time;
        setDamage(damageBase);
        setSpeed(speedBase);
        setSplash(splashBase);
    }

    public virtual void replace(Projectile p_)
    {
        transform.position = p_.transform.position;
        setSpeed(p_.getSpeedBase());
        setTarget(p_.getTarget());
        setTargetPos(p_.getTargetPos());
        setOwner(p_.getOwner());
    }

    protected virtual List<T> getAllTypesInRadius<T>(Vector3 origin, float radius) where T : Unit
    {
        List<T> objects = new List<T>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(origin.x, origin.z), radius);
        foreach(Collider2D c in colliders)
        {
            GameObject o = c.attachedRigidbody.gameObject;
            T t = o.GetComponent<T>();
            if(t != null)
                objects.Add(t);
        }
        return objects;
    }

    protected virtual Vector3 calculateVelocity()
    //DOES: Makes the projectile home on the target. Changes targetPos.
    {
		Vector3 newVel = new Vector3(targetPos.x - transform.position.x, targetPos.y - transform.position.y, targetPos.z - transform.position.z);
		Debug.Log ("newVel = " + newVel);
        newVel.Normalize();
		Debug.Log ("newVelNormalized = " + newVel);
        float scaleFactor = speed.get() * Time.fixedDeltaTime;
		Debug.Log ("scaleFactor = " +  scaleFactor);
		Debug.Log ("x = " + newVel.x * scaleFactor + ", y = " + newVel.y * scaleFactor + ", z = " + newVel.z * scaleFactor);
		newVel = new Vector3(newVel.x * scaleFactor, newVel.y * scaleFactor, newVel.z * scaleFactor);
		Debug.Log ("NewVel Final = x = " + newVel.x  + ", y = " + newVel.y + ", z = " + newVel.z );
        return newVel;
    }

    protected override void FixedUpdate()
    {
       // if(Network.isServer)
        {
            if(targetPos == transform.position || transform.position.y < -1)
            {
                destroy();
                return;
            }
            base.FixedUpdate();
            if(homing)
            {
				if(target != null)
				{
	                Vector3 pos = target.transform.position;
					targetPos = targetTrans.position;
				}
            }

			transform.position = Vector3.MoveTowards(transform.position, targetPos, speed.get()*Time.fixedDeltaTime);
            //NOTE: This can skip over enemies
        }
    }

    public virtual void OnTriggerEnter(Collider c)
    {

	    if(target != null &&  
	        ((homing && c.gameObject.GetComponent<Creep>() == target) ||
	        (!homing && c.gameObject.GetComponent<Creep>() != null))
	    )
	    {
			if(Network.isServer)
			{
		        if(splash.get() <= 0)
		        {
		            target.onDamage(getDamage());
		            if(appliedBuff != null)
		            {
		                //ADD BUFF TO TARGET HERE
		            }
		        }
		        else
		        {
		            List<Creep> victims = getAllTypesInRadius<Creep>(transform.position, splash.get());
		            foreach(Creep victim in victims)
		            {
		                victim.onDamage(getDamage());
		                if(appliedBuff != null)
		                {
		                    //APPLY BUFF HERE?
		                }
		            }
		        }
			}
	        destroy();
	    }

    }

    public virtual void destroy()
    {
		Debug.Log ("Projectile destroy");
        Destroy(this.gameObject);
    }

    protected override void Update()
    {
        base.Update();
    }

	[RPC]
	void setTargetRPC(NetworkViewID networkViewID_, NetworkMessageInfo info_)
	{
		setTarget(NetworkView.Find(networkViewID_).gameObject.GetComponent<Creep>());
	}
}
