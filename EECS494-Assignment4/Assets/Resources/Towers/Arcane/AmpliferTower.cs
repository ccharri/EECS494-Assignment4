using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmpliferTower : Tower
{
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

    protected override void fire()
    {

    }

    protected override void FixedUpdate()
    {
        GameState g = GameState.getInstance();
        if(Network.isServer)
        {
            base.FixedUpdate();
            // Cooldown elapsed, Fire!
            if((lastFired + cooldown.get()) <= g.getGameTime())
            {
                if(target == null || canFire(target) == false)
                    target = findTarget();
                if(target != null)
                    fire();
                //OPT: Increment lastFired by a deltaTime*3~ to make this faster
            }
        }
    }


}
