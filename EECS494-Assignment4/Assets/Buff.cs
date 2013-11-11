using UnityEngine;
using System.Collections;

public abstract class Buff
//NOTE: The buff isn't responsible for deleting itself!
//NOTE: Buffs aren't explicitly required to have owners!
{
	protected double duration;
	protected Unit target;
	protected Unit owner;

    public Buff(Unit target_)
    {
        target = target_;
        onApplication();
    }

    public Buff(Unit target_, Unit owner_)
    {
        target = target_;
        owner = owner_;
        onApplication();
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

    //NOTE: A buff is meaningless without a target
    private Buff() { }
}
