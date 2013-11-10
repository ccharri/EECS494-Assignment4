using UnityEngine;
using System.Collections;

public class Attribute
{
	private double baseValue;
	private double multiplier;
	private double flat;
	private double value;

	public Attribute()
	{
		baseValue = 0;
		multiplier = 0;
		flat = 0;
	}
	public Attribute(double base_)
	{
		multiplier = 0;
		flat = 0;
		baseValue = base_;
		recalculate();
	}
	public Attribute(double base_, double multiplier_, double flat_)
	{
		baseValue = base_;
		multiplier = multiplier_;
		flat = flat_;
		recalculate();
	}

	public double get() 
	{
		return value;
	}

	private void recalculate() 
	{
		value = (flat + baseValue) * multiplier;
	}

	public void setBase(double a) 		{baseValue = a;  recalculate();}
	public void setFlat(double a) 		{flat = a; 		 recalculate();}
	public void setMultiplier(double a) {multiplier = a; recalculate();}
	public double getBase() 			{return baseValue;}
	public double getFlat() 			{return flat;}
	public double getMultiplier() 		{return multiplier;}
}
