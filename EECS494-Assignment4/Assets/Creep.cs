using UnityEngine;
using System.Collections;

public class Creep : Spawnable, Selectable 
{
    public float healthBase = 1;
    public float speedBase = 1;
    public float manaBase = 1;
    public int bounty = 1;
    public int lifeCost = 1;

	public Vector3 lerpPos;

	float updateTimeStep = 0.2f;

	public Attribute health = new Attribute(1);
	public Attribute speed = new Attribute(200);
	public Attribute mana = new Attribute(1);

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

    protected bool mouseOver = false;

	PathingAgent navAgent;

    void Awake()
    {
        setHealth(healthBase);
        setSpeed(speedBase);
        setMana(manaBase);

		if(Network.isServer)
		{
			StartCoroutine("beginUpdating");
		}
    }

	public Vector3 getDestination()
	{
		return getAgent().getNextPos(transform.position);
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
		g.onCreepDeath(this);
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
		if(tag != "EndPoint") return true;
		return false;
	}
    

    protected override void FixedUpdate()
    {
		if(Network.isServer)
		{
        	base.FixedUpdate();
//        	navAgent.speed = speed.get();

			Vector3 dest = getDestination();
//            Debug.Log("dest = " + dest.x + "," + dest.z);

			transform.position = Vector3.MoveTowards(transform.position, dest, speed.get()*Time.fixedDeltaTime);
			transform.LookAt(dest);
		}
    }

	private PathingAgent getAgent()
	{
		if(navAgent == null)
		{
			navAgent = GetComponent<PathingAgent>();
		}
		return navAgent;
	}

    public string getDescription()
    {
        return "Name: " + name + "\nHealth: " + health.get() + "\nMana: " + mana.get() + "\nSpeed: " + speed.get();
    }

    public void OnMouseEnter()  { mouseOver = true; }
    public void OnMouseExit()   { mouseOver = false; }
    
    public void OnGUI()
    {
        if (mouseOver)
        {
            var x = Event.current.mousePosition.x;
            var y = Event.current.mousePosition.y;

            GUI.Label(new Rect(x - 150, y + 20, 200, 72), getDescription(), "box");
        }

        if (getHealth() < getHealthMax())
        {
            Vector3 pos = GameState.getInstance().mainCamera.WorldToScreenPoint((gameObject.transform.position + new Vector3(0, 3, 0)));
            var percent_health = getHealth() / getHealthMax();
            var texture = GameState.getInstance().hpBar_texture;

            GUI.DrawTexture(new Rect(pos.x - 8, Screen.height - pos.y, 25 * percent_health, 3), texture);
        }
    }

	public void mouseOverOn()
	{
		//TODO: Implement
	}
	public void mouseOverOff()
	{
		//TODO: Implement
	}

	IEnumerator beginUpdating()
	{
		yield return new WaitForFixedUpdate();
		Debug.Log ("Begin updating Creep Position");
		while(health.get() > 0)
		{
			networkView.RPC("update", RPCMode.Others, getDestination(), health.getFlat());
			yield return new WaitForSeconds(updateTimeStep);
		}
	}

	[RPC]
	void update(Vector3 position_, float damage_, NetworkMessageInfo info_)
	{
		Debug.Log ("Updating position = " + position_);
		lerpPos = Vector3.MoveTowards(transform.position, position_, speed.get()*Time.fixedDeltaTime*updateTimeStep);
		health.setFlat(damage_);
		StartCoroutine("lerpPosition");
	}

	IEnumerator lerpPosition()
	{
		Debug.Log ("Start Lerping to " + lerpPos);
		while(transform.position != lerpPos)
		{
			transform.position = Vector3.MoveTowards(transform.position, lerpPos, speed.get()*Time.fixedDeltaTime);
			transform.LookAt(lerpPos);
			yield return new WaitForFixedUpdate();
		}
	}

	[RPC]
	void setHealthRPC(int max)
	{
		setHealth(max);
	}

	[RPC]
	void setBountyRPC(int bounty)
	{
		setBounty(bounty);
	}

}
