using UnityEngine;
using System.Collections;
using System;

public class GameEndState : MonoBehaviour {

	private float time;
	public AlertTextBehaviour alertElement;
	public GetNameAlert nameAlert;
	public okayButton okayButton;
	private bool firstCheck = true;
	private bool timeFreeze = false;
	
	// Use this for initialization
	void Start () {
		Debug.Log("Data Path: " + Application.persistentDataPath);
		time = 0.0f;
	}

	// Update is called once per frame
	void Update () {
		if(firstCheck){
			GameStatics.Points = 140;
			int points = Convert.ToInt32(GameStatics.Points);
			Debug.Log("Points: " + points);
			string uname = getUsername();
			if(uname != string.Empty){
				addEntry(uname, points);
			}
			firstCheck = false;
			
		}
		if(!timeFreeze)
			time+= Time.deltaTime;
		//Debug.Log("Update GameEndState, Time: " + time);
		if(time > 10){
			Application.LoadLevel(0);
		}
	}
	
	public void usernameInputFinished(){
		timeFreeze = false;
		int points = Convert.ToInt32(GameStatics.Points);
		addEntry(getUsername(), points);
	}
	private string getUsername(){
		string username = GameStatics.username;
		
		if(username == string.Empty){
			nameAlert.showText = true;
			okayButton.showButton = true;
			timeFreeze = true;
		}
		return username;
		
	}
	
	private void checkHighscore(){
		StartCoroutine(
			HighscoreServer.GetHighscore(data=>{
				checkForNewHighscore(data);
			})  );
	}
	
	private void checkForNewHighscore(Entry[] data){
		
	 	int position = 1;
		foreach(var e in data){
			if(e.Points < Convert.ToInt32(GameStatics.Points))
				break;
			position++;
		}
		if(position < 4){
			
			alertElement.ShowText("New Highscore!!\n Position: " + position); //, 8, new Vector2(alertElement.positionX, alertElement.positionY));
		}
	}
	private void addEntry(string name, int points) {
		
		StartCoroutine(
			HighscoreServer.AddEntry(name, points)
		);
		checkHighscore();
	}
}
