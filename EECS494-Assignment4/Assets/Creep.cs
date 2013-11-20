using UnityEngine;
using System.Collections;

public abstract class Creep : Spawnable, Selectable 
{
	Attribute health = new Attribute(1);
	Attribute speed = new Attribute(200);
	Attribute mana = new Attribute(1);
	public int bounty = 1;
	public int lifeCost = 1;

	NavMeshAgent navAgent;

	GameState gstate;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

	public void updateDestination()
	{
		gstate = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameState>();
		navAgent = GetComponent<NavMeshAgent>();

		if (gstate.getPlayerNum(getOwner()) == 1)
		{
			navAgent.SetDestination(new Vector3(110, 0, 0));
		}
		else
		{
			navAgent.SetDestination(new Vector3(-110, 0, 0));
		}
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


    public void setHealth(float health_)    { health = new Attribute(health_); }
    public void setSpeed(float speed_)      { speed = new Attribute(speed_); }
    public void setMana(float mana_)        { mana = new Attribute(mana_); }
    public void setBounty(int bounty_)      { bounty = bounty_; }
    public void setLifeCost(int lifeCost_)  { lifeCost = lifeCost_;}

    protected override void FixedUpdate()
    {
		if(Network.isServer)
		{
        	base.FixedUpdate();
            navAgent = GetComponent<NavMeshAgent>();
        	navAgent.speed = speed.get();
		}
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
