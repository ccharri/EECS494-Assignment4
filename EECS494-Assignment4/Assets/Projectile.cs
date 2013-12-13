using UnityEngine;
using System.Collections.Generic;

public class Projectile : Unit 
{
    //External Editor Initial Settings
    public float damageBase = 10;
    public float speedBase = 7.5f;
    public float splashBase = 0;

    public bool homing = true;

    //Internal Attributes
    public Attribute speed = new Attribute(1);
    public Attribute damage = new Attribute(1);
    public Attribute splash = new Attribute(1);
    public Tower owningTower = null;
    public Creep target = null;
	public Transform targetTrans;
    public Vector3 targetPos = new Vector3(0,0,0);
    protected double birthTime = 0;

    public void setSpeed(float speed_)              { speed.setBase(speed_); }
    public void setDamage(float damage_)            { damage.setBase(damage_); }
    public void addDamage(float damage_)            { damage.setFlat(damage.getFlat() + damage_); }
    public void addDamageFactor(float damagef_)     { damage *= damagef_; }
    public void setSplash(float splash_)            { splash.setBase(splash_); }
    public void addSplash(float damage_)            { splash.setFlat(splash.getFlat() + damage_); }
    public void addSplashFactor(float splashf_)     { splash *= splashf_; }
    public void setTarget(Creep target_)            
    { 
        target = target_;
        Vector3 pos = target.gameObject.transform.position;
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
        damage = new Attribute(p_.damage);
        splash = new Attribute(p_.splash);
        speed = new Attribute(p_.speed);
        setOwner(p_.getOwner());
        setOwningTower(p_.getOwningTower());
    }

    protected virtual List<Creep> getCreepsInRadius(Vector3 origin, float radius)
    {
        List<Creep> objects = new List<Creep>();
        List<Creep> allCreeps = GameState.getInstance().getEnemyCreeps(getOwner());
        foreach(Creep c in allCreeps)
        {
            float dist = Mathf.Sqrt(Mathf.Pow(c.gameObject.transform.position.x - origin.x, 2) + 
                                     Mathf.Pow(c.gameObject.transform.position.z - origin.z, 2));
            if(dist < radius)
                objects.Add(c);
        }
        return objects;
    }

    protected virtual Vector3 calculateVelocity()
    //DOES: Makes the projectile home on the target. Changes targetPos.
    {
		Vector3 newVel = new Vector3(targetPos.x - transform.position.x, targetPos.y - transform.position.y, targetPos.z - transform.position.z);
        newVel.Normalize();
        float scaleFactor = speed.get() * Time.fixedDeltaTime;
		newVel = new Vector3(newVel.x * scaleFactor, newVel.y * scaleFactor, newVel.z * scaleFactor);
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
        }
    }

    public virtual void OnTriggerEnter(Collider c)
    {
	    if(target != null &&  
	        ((homing && c.gameObject.GetComponent<Creep>() == target) ||
	        (!homing && c.gameObject.GetComponent<Creep>() != null)))
	    {
			if(Network.isServer)
			{
		        if(splash.get() <= 0)
		        {
		            target.onDamage(getDamage());
                    applyBuff(target);
		        }
		        else
		        {
		            List<Creep> victims = getCreepsInRadius(transform.position, splash.get());
		            foreach(Creep victim in victims)
		            {
		                victim.onDamage(getDamage());
                        applyBuff(victim);
		            }
		        }
			}
	        destroy();
	    }
    }

    public virtual void applyBuff(Creep c) {}

    public virtual void destroy()
    {
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
