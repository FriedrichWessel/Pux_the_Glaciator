
using System;
using UnityEngine;
using Pux.Entities;
using Pux.Resources;

public sealed class PlayerBehaviour : EntityBehaviour
{
	public float MaxLife = 5;
	public float StartLife = 0;
	public float StartPoints = 0;
	public GameObject PenguinObject;
	public GameObject mesh;
	public GameObject hatPoint;
	public GameObject rightHandPoint;
	public GameObject beltPoint;
	public GameObject amulettPoint;
	
	public override GameObject gameObject{
		get{
			return PenguinObject;
		}
	}
	
	public float Life {
		get;
		set;
	}
	
	public float Points {
		get;
		set;
	}
	
	protected override void AwakeOverride() {
		base.AwakeOverride();
		Life = StartLife;
		Points = StartPoints;
		
		
	}
	
	protected override void StartOverride(){
		updateCloth();
	}
	public bool IsDead {
		get {return Life < 1;}
	}
	
	public void updateCloth(){
		
		changeHat();
		changeSkin();
		
		gameObject.animation.PlayQueued("happy");
		gameObject.animation.PlayQueued("show01");
	}
	private void changeHat(){
		
		var obj = GameStatics.loadPlayerHat();
		changeCloth(hatPoint, obj);
		Debug.Log("Hat: " + obj.name);
	}
	
	private void changeSkin(){
		Texture2D texture = ResourceManager.GetResource<Texture2D>("Player/Textures/"+GameStatics.PlayerSkin);
		this.mesh.renderer.material.SetTexture("_MainTex",texture);
	}
	
	private void changeCloth(GameObject hingePoint, GameObject newObject){
		
		for (int i = 0; i < hingePoint.transform.GetChildCount(); i++){
			Destroy(hingePoint.transform.GetChild(i).gameObject);
		}
		
		newObject.transform.parent = hingePoint.transform;
		newObject.transform.localPosition = Vector3.zero;
		newObject.transform.localRotation = Quaternion.identity;
		newObject.transform.localScale = new Vector3(1,1,1);
	}
}



