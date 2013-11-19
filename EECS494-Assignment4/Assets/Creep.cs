using UnityEngine;
using System.Collections;

public abstract class Creep : Spawnable, Selectable 
{
	Attribute health;
	Attribute speed;
	Attribute mana;
	public int bounty;
	public int lifeCost;

    public void Init(string name, string guid, float health_, float mana_, float speed_, int bounty_, int lifeCost_ = 1)
    {
        health = new Attribute(health_);
        mana = new Attribute(mana_);
        speed = new Attribute(speed_);
        bounty = bounty_;
        lifeCost = lifeCost_;
        Init(name, guid);
        GameState g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
        g.addCreepForPlayer(guid, this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        NavMeshAgent v = GetComponent<NavMeshAgent>();
        v.speed = speed.get();
    }

	public virtual bool onDamage(float damage)
	{
		health -= damage;
        if(!isAlive())
            onDeath();
        return isAlive();
		//NOTE: The person calling on damage needs to check if the unit isAlive after to claim kill credit.
	}
    public virtual void onDeath()
    {
        GameState g = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
        g.getEnemyCreeps(ownerGUID).Remove(this);
        Destroy(this.gameObject);
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
