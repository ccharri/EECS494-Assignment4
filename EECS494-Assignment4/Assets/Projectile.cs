﻿using UnityEngine;
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
    protected Tower owner = null;
    protected Creep target = null;
    protected Vector3 targetPos = new Vector3(0,0,0);
    protected double birthTime = 0;

    public void setSpeed(float speed_)              { speed.setBase(speed_); }
    public void setDamage(float damage_)            { damage.setBase(damage_); }
    public void setSplash(float splash_)            { splash.setBase(splash_); }
    public void setTarget(Creep target_)            { target = target_; setTargetPos(target.transform.position); }
    public void setOwner(Tower owner_)              { owner = owner_; }
    public void setTargetPos(Vector3 targetPos_)    { targetPos = targetPos_; }

    public float getSpeed()         { return speed.get(); }
    public float getSpeedBase()     { return speed.getBase(); }
    public float getDamage()        { return damage.get(); }
    public float getSplash()        { return splash.get(); }
    public Creep getTarget()        { return target; }
    public Tower getOwner()         { return owner; }
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

    protected virtual Vector3 calculateVelocity()
    //DOES: Makes the projectile home on the target. Changes targetPos.
    {
        Vector3 newVel = (targetPos - transform.position);
        newVel.Normalize();
        float scaleFactor = (float)speed.get() * Time.fixedDeltaTime;
		newVel *= scaleFactor;
        return newVel;
    }
    protected override void FixedUpdate()
    {
        if(Network.isServer)
        {
            if(target == null)
                Network.Destroy(this.gameObject);

            base.FixedUpdate();
            if(homing)
                targetPos = target.transform.position;
            transform.rigidbody.transform.position += calculateVelocity();
            //NOTE: This can skip over enemies
        }
    }
    public virtual void OnTriggerEnter(Collider c)
    {
        if(Network.isServer)
        {
            if(target != null && c.gameObject.GetComponent<Creep>() == target)
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

                }
                destroy();
            }
        }
    }

    public virtual void destroy()
    {
		Debug.Log ("Projectile destroy");
        Network.Destroy(this.gameObject);
    }

    protected override void Update()
    {
        base.Update();
    }
}
