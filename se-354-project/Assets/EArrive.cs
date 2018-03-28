using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EArrive : MonoBehaviour {

	public Vector3 target;
	private Vector3 velocity;
	private int maxSpeed = 5;
	private int maxAcc = 3; 
	private float slowRadius; 
	private float radius = 0.1f; 

	// Use this for initialization
	void Start () {
		target = transform.position;
		velocity = new Vector3 (0, 0, 0);
		// computing slow radius: 
		// time to come to full stop from maxSpeed is maxSpeed / maxAcc
		// during this time, the average speed is (maxSpeed+0)/2
		// to stop, we need maxSpeed/2 * (maxSpeed / maxAcc) 
		slowRadius = ((float)maxSpeed / (float)maxAcc) * ((float)maxSpeed / 2.0f); 
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			target = Input.mousePosition;
		}
		if ((target - transform.position).magnitude > radius) {
			Vector3 direction = target - transform.position;
			float distance = direction.magnitude;
			direction.Normalize ();

			float speed = 0;
			if (distance > slowRadius) {
				// we can go full speed
				speed = maxSpeed;
			} else {
				// distance / slowRadius will always be less than 1, moving towards 0. 
				speed = maxSpeed * (distance / slowRadius);
			}

			Vector3 targetAcc = (direction * speed) - velocity;
			if (targetAcc.magnitude > maxAcc) {
				targetAcc = targetAcc.normalized * maxAcc;
			}

			velocity += targetAcc * Time.deltaTime;
			if (velocity.magnitude > maxSpeed) {
				velocity = velocity.normalized * maxSpeed;
			}

			// rotate
			float a = Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg;


			//transform.rotation = Quaternion.Euler (0, a, 0);
			//GameObject.Find("tank").GetComponent<DynamicAlign>().target = a;


			transform.Translate (velocity * Time.deltaTime, Space.World);
		} else {
			velocity = new Vector3 (0, 0, 0);
		}
	}
}
