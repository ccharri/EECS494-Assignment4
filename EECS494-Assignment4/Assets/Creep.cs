using UnityEngine;
using System.Collections;

public abstract class Creep : Spawnable, Selectable 
{
	Attribute health;
	Attribute speed;
	Attribute mana;
	int bounty;
	int lifeCost;

    public Creep()
    {
        tag = "Creep";
    }
    public Creep(double health_, double mana_, double speed_, int bounty_, int lifeCost_ = 1)
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
        return isAlive();
		//NOTE: The person calling on damage needs to check if the unit isAlive after to claim kill credit.
	}
    public abstract void onDeath();


	public virtual bool isAlive()
	{
		return health.get() > 0;
	}


	public abstract string getDescription();
	public void mouseOverOn()
	{
		//TODO: Implement
	}
	public void mouseOverOff()
	{
		//TODO: Implement
	}
}
