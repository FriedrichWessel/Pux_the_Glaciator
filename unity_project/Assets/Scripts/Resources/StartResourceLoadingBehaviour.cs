using System;
using UnityEngine;
using asdf.Resources;


public sealed class StartResourceLoadingBehaviour : ResourceLoadingBehaviour
{
	
	private Timer timer;
	
	public StartResourceLoadingBehaviour() {
	}
	protected override void OnResourcesLoaded(EventArgs e) {
		base.OnResourcesLoaded(e);
		
		//Application.LoadLevel("MainMenu");
	}
	
	void Awake(){
		timer = new Timer(4);
		timer.TimerFinished += OnTimerFinished;
	}
	
	void Start(){
		timer.StartTimer(4);
		
	}
	
	void OnTimerFinished(object sender, EventArgs e){
		Application.LoadLevel("MainMenu");
	}
	
	

}


