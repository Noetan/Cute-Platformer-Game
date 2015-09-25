using UnityEngine;
using System.Collections;

public class OceanTexMovement : MonoBehaviour {
	
	public Vector2 UvSpeed = new Vector2(0.05f, 0.15f);
	public bool hasMainMap = true;
	public bool hasBumpMap = true;
	
	private float offset1 = 0.0f;
	private float offset2 = 0.0f;
	
	// Update is called once per frame
	void Update () {
		offset1 = Time.time * UvSpeed.x;
		offset2 = Time.time * UvSpeed.y;
		if(hasMainMap)
			GetComponent<Renderer>().material.SetTextureOffset ("_MainTex", new Vector2(-offset1, -offset2));
		if(hasBumpMap)
			GetComponent<Renderer>().material.SetTextureOffset ("_BumpMap", new Vector2(-offset1, -offset2));
	}
}
