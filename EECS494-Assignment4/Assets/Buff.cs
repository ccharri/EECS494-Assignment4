using UnityEngine;
using System.Collections;

public abstract class Buff : MonoBehaviour
//NOTE: Buff is either a factory class or an actual Buff.
//      A buff without a target is automatically a factoy class.
//      This doesn't mean that an active buff can't be a factory.
{
    private bool buffEnabled;
	public float duration;
    public int level;
    public Unit owner;
    public GameObject effect;

    public void setEnabled(bool enabled_)       { buffEnabled = enabled_; }
    public void setDuration(float duration_)    { duration = duration_; }
    public void setOwner(Unit owner_)           { owner = owner_; }
    public void setLevel(int level_)            { level = level_; }

    public bool getEnabled()    { return buffEnabled; }
    public float getDuration()  { return duration; }
    public Unit getOwner()      { return owner; }
    public int getLevel()       { return level; }
    

    public virtual void Awake()
    {
        buffEnabled = false;
    }

    public virtual void Init(int level_)
    //DOES: Sets up the intial attributes of the buff
    {
        level = level_;   
    }

    public virtual void onApplication() 
    //DOES: Called when the buff should apply to the unit
    {
        buffEnabled = true;
    }
    
    public virtual void onProjectile(Projectile p) {} 
    //DOES: Called by anything which launches a projectile

    public virtual void OnDestroy()
    //DOES: Called when the buff needs to be cleaned up
    {
        Destroy(effect);
    }

    public virtual void FixedUpdate()
    {
        if(!buffEnabled)
            return;

        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            Destroy(this);
        }

        if(effect != null)
            effect.transform.position = gameObject.transform.position;
    }
}
