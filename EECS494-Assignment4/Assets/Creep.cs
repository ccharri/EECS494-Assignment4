using UnityEngine;
using System.Collections;

public abstract class Creep : Spawnable, Selectable 
{
	Attribute health;
	Attribute speed;
	Attribute mana;
	int bounty;

	public void onDamage()
	{


	}
	public bool isAlive()
	{
		return health.get() > 0;
	}

	public string getDescription()
	{
		//TODO: Implement
		return "REPLACE ME?";
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
