using UnityEngine;
using System.Collections;

public abstract class Buff
//NOTE: The buff isn't responsible for deleting itself!
//NOTE: Buffs aren't explicitly required to have owners!
//NOTE: Targets are stored within the second level Buffs!
{
	protected double duration;
    protected Unit owner;

    public Buff() {}

    public Buff(Unit owner_)
    {
        owner = owner_;
    }

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
