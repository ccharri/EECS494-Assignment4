using UnityEngine;
using System.Collections;

[RequireComponent(typeof (NavMeshAgent))]

public class Creep : Spawnable, Selectable 
{
    public float healthBase = 1;
    public float speedBase = 1;
    public float manaBase = 1;
    public int bounty = 1;
    public int lifeCost = 1;

	protected Attribute health = new Attribute(1);
	protected Attribute speed = new Attribute(200);
	protected Attribute mana = new Attribute(1);

    public void setHealth(float health_)    { health = new Attribute(health_); }
    public void setSpeed(float speed_)      { speed = new Attribute(speed_); }
    public void setMana(float mana_)        { mana = new Attribute(mana_); }
    public void setBounty(int bounty_)      { bounty = bounty_; }
    public void setLifeCost(int lifeCost_)  { lifeCost = lifeCost_; }

    public float getHealth()    { return health.get(); }
    public float getHealthMax() { return health.getBase(); }
    public float getSpeed()     { return speed.get(); }
    public float getMana()      { return mana.get(); }
    public int getBounty()      { return bounty; }
    public int getLifeCost()    { return lifeCost; }

	NavMeshAgent navAgent;

    void Awake()
    {
        setHealth(healthBase);
        setSpeed(speedBase);
        setMana(manaBase);
        navAgent = GetComponent<NavMeshAgent>();
		navAgent.speed = 0;
    }

	public Vector3 getDestination()
	{
		GameState gstate = GameState.getInstance();

		bool sendToPlayerTwo = (Network.isClient) ||(Network.isServer && (getOwner() != Network.player.guid));

		if (!sendToPlayerTwo)
		{
			return new Vector3(43.8f, 0, 0);
		}
		else
		{
			return new Vector3(-43.8f, 0, 0);
		}
	}

	public void updateDestination()
	{
		getAgent().SetDestination(getDestination());
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
		if(Network.isClient) return;

		GameState g = GameState.getInstance();
		g.removeCreep(networkView.viewID, g.getPlayer(getOwner()));
		g.networkView.RPC("removeCreep", RPCMode.OthersBuffered, networkView.viewID,  g.getPlayer(getOwner()));

        Network.Destroy(this.gameObject);
    }

    public virtual bool isAlive()
    {
        return health.get() > 0;
    }

	void OnCollisionEnter(Collision info)
	{
		if(shouldFilter(info.gameObject.tag)) return;
		
		GameState.getInstance().onCreepLeaked(this);
	}

	void OnTriggerEnter(Collider info)
	{
		if(shouldFilter(info.gameObject.tag)) return;
		
		GameState.getInstance().onCreepLeaked(this);
	}

	bool shouldFilter(string tag)
	{
		if(tag == "EndPoint") return true;
		if(tag == "Projectile") return true;
		return false;
	}
    

    protected override void FixedUpdate()
    {
		if(Network.isServer)
		{
        	base.FixedUpdate();
//        	navAgent.speed = speed.get();

			Vector3 dest = getAgent().path.corners[0];

			transform.position += (dest - transform.position).normalized * speed.get () * Time.fixedDeltaTime;
		}
    }

	private NavMeshAgent getAgent()
	{
		if(navAgent == null)
		{
			navAgent = GetComponent<NavMeshAgent>();
		}
		return navAgent;
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
