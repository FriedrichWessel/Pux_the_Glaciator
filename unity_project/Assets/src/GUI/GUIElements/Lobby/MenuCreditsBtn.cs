using UnityEngine;
using System.Collections;

public class MenuCreditsBtn : GUIBaseElement<GUIStatics> {
		
	
	protected override void showElements(){
		GeneralScreenGUI.Box(guiStatics, new Rect (positionX,positionY,256,256), "", currentStyle);
	}
	
	protected override void hit(){
		Debug.Log("Credits not Implemented YET!");

	}
}