using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWASD : MonoBehaviour {
	private int speed = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private void Update() {

		transform.Translate (Input.GetAxis ("Horizontal") * speed * Time.deltaTime, 0f, Input.GetAxis("Vertical")*Time.deltaTime*speed);

	}
}
