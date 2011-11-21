using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UVMoveBehaviour : MonoBehaviour {

	public Rect newUvs{
		get{
			return currentUvs;
		}
		set{
			currentUvs = value;
			updateCurrentUV();
		}
	}
	
	public bool AbsoluteUVPosition = false;
	
	protected Texture mainTexture;
	
	private Rect currentUvs;
	private MeshFilter meshFilter;
	private List<Vector2> originalUVs;
	private bool disabled = true;
	
	void Start(){
		StartOverride();
	}
	
	protected virtual void StartOverride(){
		if(mainTexture == null)
			initMainTexture();
	}
	void Awake(){
		AwakeOverride();
	}
	
	protected virtual void AwakeOverride(){
		initMainTexture();
		newUvs = new Rect(0,0,0,0);
		disabled = false;
		loadMesh();
		if(!disabled)
			storeUvs();
	}
	
	private void initMainTexture(){
		mainTexture = meshFilter.renderer.material.GetTexture("_MainTex");
		if(t == null){
			Debug.LogWarning("No Texture set for UVMoveBehaviour on: " + gameObject.name);
			return xy;
		}
	}
	
	private void storeUvs(){
		var uvs = meshFilter.mesh.uv;
		originalUVs = new List<Vector2>();
		for(int i=0; i< uvs.Length; i++){
			Vector2 uv = uvs[i];
			originalUVs.Add(uv);
		}
		
	}
	private void loadMesh(){
		meshFilter = gameObject.GetComponent<MeshFilter>() as MeshFilter;
		if(meshFilter == null){
			meshFilter = gameObject.GetComponentInChildren<MeshFilter>() as MeshFilter;
		}
		if(meshFilter == null){
			Debug.LogError("No Meshfound for UVMoveBehaviour on Object: " + gameObject.name);
			disabled = true;
			
		}
		
	}
	
	private void updateCurrentUV(){
		if(disabled)
			return;
		
		var uvs = meshFilter.mesh.uv;
		for(int i=0; i< originalUVs.Count; i++){
			
			Vector2 uv = originalUVs[i];
			Vector2 normPos = toUVSpace(new Vector2(newUvs.x, newUvs.y));
			if(AbsoluteUVPosition){
				uv.x = normPos.x;
				uv.y = normPos.y;
				float factorX = newUvs.width / mainTexture.width;
				float factorY =  newUvs.height / mainTexture.height;
				uv.x *= factorX;
				uv.y *= factorY;
				
			} else {
				uv.x += normPos.x;
				uv.y += normPos.y;
				uv.x *= newUvs.width;
				uv.y *= newUvs.height;
			}
			
				
			uvs[i] = uv;
			
		}
		meshFilter.mesh.uv = uvs;
	}
	
	private Vector2 toUVSpace(Vector2 xy){
		if(Math.Abs(xy.x) < 1 && Math.Abs(xy.y) < 1)
			return xy;
			
		if(mainTexture == null)
			initMainTexture();
		
		var p = new Vector2(xy.x / ((float)mainTexture.width), xy.y / ((float)mainTexture.height));
		return p;
	}

	
}
