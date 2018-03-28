using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//GET THE POINT OF WHERE I CLIKCK AND SET THAT POINT AS A TARGET

public class FixedCameraWithMouseControls : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			RaycastHit h;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
			if (Physics.Raycast (ray, out h)) {
				Vector3 target = h.point;
				target.y = 0.5f; // let's ignore y 
				GameObject.Find("tank").GetComponent<EArrive>().target = target;
			}
		}
	}
}
