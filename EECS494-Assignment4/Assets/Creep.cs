using UnityEngine;
using System.Collections;

public class Creep : Spawnable, Selectable 
{
	Attribute health;
	Attribute speed;
	Attribute mana;
	public int bounty;
	public int lifeCost;

    public void Init(double health_, double mana_, double speed_, int bounty_, int lifeCost_ = 1)
    {
        health = new Attribute(health_);
        mana = new Attribute(mana_);
        speed = new Attribute(speed_);
        bounty = bounty_;
        lifeCost = lifeCost_;
    }

	public virtual bool onDamage(double damage)
	{
		health -= damage;
        if(!isAlive())
            onDeath();
        return isAlive();
		//NOTE: The person calling on damage needs to check if the unit isAlive after to claim kill credit.
	}
    public virtual void onDeath()
    {
        Destroy(this);
    }

	public virtual bool isAlive()
	{
		return health.get() > 0;
	}


    public string getDescription()
    {
        return "Health: " + health.get() + "\nMana: " + mana.get() + "\nSpeed: " + speed.get();
    }
	public void mouseOverOn()
	{
		//TODO: Implement
	}
	public void mouseOverOff()
	{
		//TODO: Implement
	}
}
