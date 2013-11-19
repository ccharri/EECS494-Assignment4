using UnityEngine;
using System.Collections;

public class Attribute
{
	private float baseValue;
	private float multiplier;
	private float flat;
	private float value;

	public Attribute()
	{
		baseValue = 0;
		multiplier = 1;
		flat = 0;
	}
	public Attribute(float base_)
	{
		multiplier = 1;
		flat = 0;
		baseValue = base_;
		recalculate();
	}
	public Attribute(float base_, float multiplier_, float flat_)
	{
		baseValue = base_;
		multiplier = multiplier_;
		flat = flat_;
		recalculate();
	}

	public float get() 
	{
		return value;
	}

    private void recalculate()
    {
        value = (flat + baseValue) * multiplier;
    }

    //NOTE: These functions modify the COMPONENTS of Attribute, not THE ACTUAL VALUE
    //      Think of them like STAT BONUSES, especially for * and /
    //      * : +x%     + : +x
    //      / : -x%     - : -x
    public static Attribute operator -(Attribute a, float b)
    {
        return new Attribute(a.getBase(), a.getMultiplier(), a.getFlat() - b);
    }
    public static Attribute operator +(Attribute a, float b)
    {
        return new Attribute(a.getBase(), a.getMultiplier(), a.getFlat() + b);
    }
    public static Attribute operator *(Attribute a, float b)
    {
        return new Attribute(a.getBase(), a.getMultiplier()+b, a.getFlat());
    }
    public static Attribute operator /(Attribute a, float b)
    {
        return new Attribute(a.getBase(), a.getMultiplier()-b, a.getFlat());
    }

	public void setBase(float a) 		{baseValue = a;  recalculate();}
	public void setFlat(float a) 		{flat = a; 		 recalculate();}
	public void setMultiplier(float a)  {multiplier = a; recalculate();}
	public float getBase() 			    {return baseValue;}
	public float getFlat() 			    {return flat;}
	public float getMultiplier() 		{return multiplier;}
}
