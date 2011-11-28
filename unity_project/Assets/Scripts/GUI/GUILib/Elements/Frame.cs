using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Pux.Controllers;

public class Frame : MonoBehaviour
{
	protected List<Panel> directChildren;
	protected delegate void InteractionEvent(InteractionBehaviour ib);
	protected delegate void ActionEvent(Frame b);

	// DONT USE THIS!
	void Awake() {
		AwakeOverride();
	}

	// Use this for initialization
	protected virtual void AwakeOverride() {
		initDirectChildren();
	}

	void Start() {
		
	}
	
	// Update is called once per frame
	void Update() {
		UpdateOverride();
	}

	protected virtual void UpdateOverride() {
		// nothing here
	}

		
	/**
	 * This Function is called by Parent to force the child to arrange them selves 
	 **/
	public virtual void LayoutElement() {
		
		
		//do positioning etc. for this class here
	}

	public virtual void OnClick(object sender, MouseEventArgs e) {
		
		callHandler(ib => { ib.Click(e); }, action => { action.OnClick(sender, e); });
	}

	public virtual void OnHover(object sender, MouseEventArgs e) {
		callHandler(ib => { ib.Hover(e); }, action => { action.OnHover(sender, e); });
	}

	public virtual void OnDown(object sender, MouseEventArgs e) {
		callHandler(ib => { ib.Down(e); }, action => { action.OnDown(sender, e); });
	}

	public virtual void OnUp(object sender, MouseEventArgs e) {
		callHandler(ib => { ib.Up(e); }, action => { action.OnUp(sender, e); });
	}

	public virtual void OnMove(object sender, MouseEventArgs e) {
		callHandler(ib => { ib.Move(e); }, action => { action.OnMove(sender, e); });
	}

	public virtual void OnSwipe(object sender, MouseEventArgs e) {
		callHandler(ib => { ib.Swipe(e); }, action => { action.OnSwipe(sender, e); });
	}

	protected virtual void callHandler(InteractionEvent interaction, ActionEvent action) {
		foreach (Frame b in directChildren) {
			if(b == null)
				continue;
			if (b.checkMouseOverElement()) {
				if (action != null) {
					action(b);					
				}
				var behaviours = b.GetComponents<InteractionBehaviour>() as InteractionBehaviour[];
				if (behaviours != null) {
					foreach (var ib in behaviours) {
						interaction(ib);
					}	
				}
			} else {
				b.resetElement();
			}
		}
	}
	
	public virtual bool checkMouseOverElement(){
		return true;
	}
	
	public void UpdateDirectChildren() {
		initDirectChildren();
	}
	
	public virtual void UpdateElement(){
		UpdateDirectChildren();
		foreach (Panel panel in directChildren){
			panel.UpdateElement();
		}	
	}
	
	public virtual void CreateElement(){
		UpdateDirectChildren();
		foreach (Panel panel in directChildren)
			panel.CreateElement();
	}
	
	public virtual void resetElement(){
		
	}
	
	private void initDirectChildren() {
		directChildren = new List<Panel>();
		foreach (Transform child in transform) {
			var b = child.GetComponent<Panel>();
			if (b != null)
				directChildren.Add(b);
		}
	}
}
