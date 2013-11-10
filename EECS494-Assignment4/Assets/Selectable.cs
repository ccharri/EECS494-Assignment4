using UnityEngine;
using System.Collections;

public interface Selectable 
//DOES: Interface for any object which can be selected by a player.
{
	string getDescription();
	void mouseOverOn();
	void mouseOverOff();
}
