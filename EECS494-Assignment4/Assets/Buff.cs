using UnityEngine;
using System.Collections;

public abstract class Buff<T> where T : Unit
//NOTE: The buff isn't responsible for deleting itself!
//NOTE: Buffs aren't explicitly required to have owners!
//NOTE: Buffs DO NOT add themselves to the target (yet?)
{
	protected double duration;
    protected Unit owner;
    protected T target;

    public Buff(T target_)
    {
        target = target_;
        onApplication();
    }
    public Buff(T target_, Unit owner_)
    {
        target = target_;
        owner = owner_;
        onApplication();
    }

    //TODO: Make onApplication automatically add the buff to the unit
    public abstract void onApplication();
    public abstract void onRemoval();

    public virtual bool FixedUpdate()
    //DOES: Returns if the buff has expired or not
    {
        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            onRemoval();
            return true;
        }
        return false;
    }
}
