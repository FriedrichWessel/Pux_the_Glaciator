using UnityEngine;
using System.Collections;
using System;

public class WardrobeBoxBehaviour : Button {
	
	public TextPanel InfoTafel;
	public int points;
	public string infoText;
	public string HatName;
	
	public string help2 = "ONLY FOR EditorDebug USE:";
	public bool Locked = false;
	public bool Unlocked = false;

	public Texture2D Skin;
	
	public Rect UV_locked;
	public Rect UV_locked_active;
	
	
	private string currentText;
	
	public event EventHandler PlayerClothChanged;
	
	void Start(){
		init();
	}

	
	protected virtual void init(){
		if(!Unlocked &&  ( (int)GameStatics.PersonalHighscore < points || Locked ) ){
			currentText = "you need " + points + " points \nto unlock this items";
			Uv = UV_locked;
			hoverUV = UV_locked_active;
			activeUV = UV_locked_active;
		} else {
			currentText = infoText;
		}
	}
	
	public override void OnClick(object sender, MouseEventArgs e){
		EditorDebug.Log("ChangePlayerCloth");
		changePlayerCloth();
		InvokePlayerClothChanged();
		if(InfoTafel != null)
			InfoTafel.Text = currentText;
		else
			EditorDebug.LogWarning("Wardrobebox: " + gameObject.name + " has no infoField Set!");
	}
	
	
	
	protected virtual void changePlayerCloth(){
		loadHat();
		loadSkin();
		
		
	}
	
	private void loadHat(){
		if(GameStatics.PersonalHighscore >=  points){
			GameStatics.savePlayerHat(HatName);
		
		}
			
		
	}
	
	private void loadSkin(){
		if(GameStatics.PersonalHighscore >=  points){
			GameStatics.PlayerSkin = Skin.name;	
		}
	}
	private void InvokePlayerClothChanged(){
		EditorDebug.Log("INVOKE");
		var handler = PlayerClothChanged;
		if (handler == null) {
				return;
		}
		handler(this, EventArgs.Empty);
	}
}
