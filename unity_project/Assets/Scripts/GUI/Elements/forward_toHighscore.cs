using UnityEngine;
using System.Collections;

public class forward_toHighscore : UIElementBehaviour<GUIStatics> {

	protected override void showElements(){
		GeneralScreenGUI.Box(guiStatics, new Rect(positionX,positionY,256,256), "", currentStyle);
		
	}
	
	protected override void hit(){
		Application.LoadLevel(5);
	}
}
