using UnityEngine;
using System.Collections;

public class DummyCreep : Creep {

    private string toolTipText = "";
    private GUIStyle guiStyleFore;
    private GUIStyle guiStyleBack;


	// Use this for initialization
	void Start () {
      guiStyleFore = new GUIStyle();
      guiStyleFore.normal.textColor = Color.white;
      guiStyleFore.alignment = TextAnchor.UpperLeft;
      guiStyleFore.wordWrap = true;

      guiStyleBack = new GUIStyle();
      guiStyleBack.normal.textColor = Color.black;
      guiStyleBack.alignment = TextAnchor.UpperLeft;
      guiStyleBack.wordWrap = true;		
	}

    public override void Init(string s)
    {
        
    }

  public string getDescription()
  {
      return "DummyCreep description";
  }

  public void OnMouseEnter()
  {
      toolTipText = getDescription();
      System.Console.WriteLine("on mouse enter!!!");
  }

  public void OnMouseExit()
  {
      toolTipText = "";
  }

  public void OnGUI()
  {
      if (toolTipText != "")
      {
          var x = Event.current.mousePosition.x;
          var y = Event.current.mousePosition.y;

          var rectBack = new Rect(x - 99, y + 21, 300, 60);
          var rectFore = new Rect(x - 100, y + 20, 300, 60);

          GUI.Box(rectBack, new GUIContent());

          GUI.Label(rectBack, toolTipText, guiStyleBack);
          GUI.Label(rectFore, toolTipText, guiStyleFore);
      }
  }
	
	// Update is called once per frame
	void Update () {
	
	}
}
