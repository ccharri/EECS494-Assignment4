using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections.Generic;

public abstract class Tower: Spawnable, Selectable
{
    //External Editor Attributess
    public float rangeBase = 10;
    public float cooldownBase = 1;

    public string description = "";
    public bool selected = false;

    //Internal Attribues
    protected Attribute range = new Attribute(1);
    protected Attribute cooldown = new Attribute(1);

    //Internal Book-Keeping
    protected double lastFired = 0;
    protected bool mouseOver = false;

    public void setRange(float range_) { range.setBase(range_); }
    public void setCooldown(float cooldown_) { cooldown.setBase(cooldown_); }

    public float getRange() { return range.get(); }
    public float getCooldown() { return cooldown.get(); }

    protected virtual void Awake()
    {
        setRange(rangeBase);
        setCooldown(cooldownBase);
        selected = false;
    }

    protected override void Update() { base.Update(); }
    protected override void FixedUpdate() { base.FixedUpdate(); }

    public abstract string getDescription();

    public void OnGUI()
    {
        if(mouseOver)
        {
            var x = Event.current.mousePosition.x;
            var y = Event.current.mousePosition.y;

            GUI.Label(new Rect(x - 150, y + 20, 200, 72), getDescription(), "box");


            if(Input.GetMouseButtonDown(0))
            {
                selected = true;
            }
        }
        else
        {
            selected = false;
        }
    }

    public virtual void mouseOverOn() { }
    public virtual void mouseOverOff() { }
}

