using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnGUI()
  {
      GUILayout.BeginArea(new Rect(10, 5, Screen.width - 15, Screen.height - 5));
      GUILayout.BeginVertical();

      OnGUI_TopBar();
      OnGUI_ScoreBoard();
      GUILayout.FlexibleSpace();
      OnGUI_BottomBar();

      GUILayout.EndVertical();
      GUILayout.EndArea();
  }

  void OnGUI_TopBar()
  {
      GUILayout.BeginHorizontal(GUILayout.Height(20));

      var layoutOptions = new GUILayoutOption[] { GUILayout.Height(20) };

      GUILayout.Button("FILE", layoutOptions);
      GUILayout.Button("EDIT", layoutOptions);

      GUILayout.FlexibleSpace();

      var currIncomeTimer = 5;            //<call for this data later>
      var incomeTimerString = "Income Timer: " + currIncomeTimer;
      GUILayout.Label(incomeTimerString, layoutOptions);

      GUILayout.FlexibleSpace();

      var currGold = 10;                //Find this data
      var currIncome = 2;               //ditto
      var goldIncomeString = "Gold + Income: " + currGold + " + " + currIncome;
      GUILayout.Label(goldIncomeString, layoutOptions);

      GUILayout.EndHorizontal();
  }

  void OnGUI_ScoreBoard()
  {
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();

      var hostPlayerLives = 5;      //TODO: get these
      var clientPlayerLives = 9;
      var scoreBoardString = "Player 1 Lives: " + hostPlayerLives + "\nPlayer 2 Lives: " + clientPlayerLives;
      GUILayout.Box(scoreBoardString);

      GUILayout.EndHorizontal();
  }

  void OnGUI_BottomBar()
  {
      GUILayout.BeginHorizontal();

      GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 5));
      GUILayout.Label("Buffs: ");
      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();

      //TOWER PLACEMENT BUTTONS
      GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 5));
      GUILayout.BeginHorizontal();
      GUILayout.Button("Tower1");
      GUILayout.Button("Tower2");
      GUILayout.Button("Tower3");
      GUILayout.Button("Tower4");
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Button("Tower5");
      GUILayout.Button("Tower6");
      GUILayout.Button("Tower7");
      GUILayout.Button("Tower8");
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();

      GUILayout.FlexibleSpace();

      //CREEP PLACEMENT BUTTONS
      GUILayout.BeginVertical(GUILayout.Width(Screen.width / 4), GUILayout.Height(Screen.height / 5));
      GUILayout.BeginHorizontal();
      GUILayout.Button("Creep1");
      GUILayout.Button("Creep2");
      GUILayout.Button("Creep3");
      GUILayout.Button("Creep4");
      GUILayout.EndHorizontal();

      GUILayout.BeginHorizontal();
      GUILayout.Button("Creep5");
      GUILayout.Button("Creep6");
      GUILayout.Button("Creep7");
      GUILayout.Button("Creep8");
      GUILayout.EndHorizontal();

      //Upgrade Buttons
      GUILayout.FlexibleSpace();
      GUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();
      GUILayout.Button("Upgrade Creeps");
      GUILayout.EndHorizontal();
      GUILayout.EndVertical();

      GUILayout.EndHorizontal();
  }
}
