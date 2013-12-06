using UnityEngine;
using System.Collections;

public abstract class Buff : MonoBehaviour
//NOTE: Buff is either a factory class or an actual Buff.
//      A buff without a target is automatically a factoy class.
//      This doesn't mean that an active buff can't be a factory.
{
    private bool enabled;
	public float duration;
    public int level;
    public Unit owner;

    public void setEnabled(bool enabled_)       { enabled = enabled_; }
    public void setDuration(float duration_)    { duration = duration_; }
    public void setOwner(Unit owner_)           { owner = owner_; }
    public void setLevel(int level_)            { level = level_; }

    public bool getEnabled()    { return enabled; }
    public float getDuration()  { return duration; }
    public Unit getOwner()      { return owner; }
    public int getLevel()       { return level; }
    

    public virtual void Awake()
    {
        enabled = false;
    }

    public virtual void Init(int level_)
    {
        level = level_;   
    }

    public abstract void onRemoval();
    public virtual void onApplication()
    {
        enabled = true;
    }

    public virtual void FixedUpdate()
    {
        if(!enabled)
            return;

        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            onRemoval();
        }
    }
}
