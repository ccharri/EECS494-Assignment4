using UnityEngine;
using System.Collections;

public abstract class Creep : Spawnable, Selectable 
{
	Attribute health;
	Attribute speed;
	Attribute mana;
	int bounty;
	int lifeCost;


	public virtual void onDamage(double damage)
	{
		health -= damage;
		//NOTE: The person calling on damage needs to check if the unit isAlive after to claim kill credit.
	}
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
