using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Amplifer : Spawnable, Selectable 
{
    //External Editor Attributess
    public float rangeBase = 10;
    public float cooldownBase = 1;

    public string buffApplied = "Damage";
    public int buffAppliedLevel = 1;

    public string description = "";

    //Internal Attribues
    protected Attribute range = new Attribute(1);
    protected Attribute cooldown = new Attribute(1);

    //Internal Book-Keeping
    protected TargetingBehaviorProjectile behavior = Closest.getInstance();
    protected double lastFired = 0;

    protected bool mouseOver = false;

    public void setRange(float range_) { range.setBase(range_); }
    public void setCooldown(float cooldown_) { cooldown.setBase(cooldown_); }

    public float getRange() { return range.get(); }
    public float getCooldown() { return cooldown.get(); }

    void Awake()
    {
        setRange(rangeBase);
        setCooldown(cooldownBase);
    }

    protected virtual List<Projectile> getAllProjectilesInRadius(Vector3 origin, float radius)
    {
        List<Projectile> objects = new List<Projectile>();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(origin.x, origin.z), radius);
        foreach(Collider2D c in colliders)
        {
            GameObject o = c.attachedRigidbody.gameObject;
            Projectile p = o.GetComponent<Projectile>();
            if(p != null && p.getOwner() == getOwner())
                objects.Add(p);
        }
        return objects;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        GameState g = GameState.getInstance();
        if(Network.isServer)
        {
            base.FixedUpdate();
            // Cooldown elapsed, Fire!
            if((lastFired + cooldown.get()) <= g.getGameTime())
                fire();
        }
    }

    protected virtual void fire()
    {
        Projectile p = findTarget();
        Buff b = p.gameObject.AddComponent(buffApplied) as Buff;
        b.Init(buffAppliedLevel);
        b.onApplication();
        //b.effect = Instantiate(sufferingEffect, buff.gameObject.transform.position, new Quaternion()) as GameObject;
        lastFired = GameState.getInstance().getGameTime();
    }

    protected virtual Projectile findTarget()
    {
        List<Projectile> projectiles = getAllProjectilesInRadius(gameObject.transform.position, range.get());
        if(projectiles.Count == 0)
            return null;
        Projectile newTarget = null;
        foreach(Projectile p in projectiles)
        {
            if(newTarget == null)
                newTarget = p;
            else if(behavior.compare(p, newTarget, this))
                newTarget = p;
        }
        return newTarget;
    }

    public virtual string getDescription()
    {
        return "Name: " + name + "\nDamage: +10" + "\nRange: " + rangeBase + "\nCooldown: " + cooldownBase;
    }

    public void OnMouseEnter() { mouseOver = true; }
    public void OnMouseExit() { mouseOver = false; }

    public void OnGUI()
    {
        if(mouseOver)
        {
            var x = Event.current.mousePosition.x;
            var y = Event.current.mousePosition.y;

            GUI.Label(new Rect(x - 150, y + 20, 200, 72), getDescription(), "box");
        }
    }

    public virtual void mouseOverOn() { }
    public virtual void mouseOverOff() { }
}
