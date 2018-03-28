using UnityEngine;
using System.Collections;

public class DynamicAlign : MonoBehaviour {
	public float target;
	public float speed;

	public int maxAngAcc = 10;
	public int maxRotSpd = 30;
	public float targetRadius = 0.1f;
	public float slowRadius   = 5.0f;
	public float timeToTarget = 0.1f;

	public bool isDone=false;
	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		/* Get current orientation on the y-axis */
		float a = transform.eulerAngles.y;
		/* Calculate the angle to the target */
		float r = Mathf.DeltaAngle (a, target);
		/* The value of r can be negative, take its magnitude (abs) */
		float rotationSize = Mathf.Abs (r);
		if (rotationSize < targetRadius) {
			/* We are within an acceptable margin. */
			return;
		}

		/* Similar to arriving, let's create a targetRotation */
		float targetRotation = 0.0f;
		if (rotationSize > slowRadius) {
			/* We still have angles to cover. */
			targetRotation = maxRotSpd;
		} else {
			targetRotation = maxRotSpd * rotationSize / slowRadius;
		}
		/* Let's get the direction (cw or ccw) */
		targetRotation *= r / rotationSize; /* r/rotationSize is either 1 or -1 */

		/* Similar to arrival, let's try to get to the target rotation */
		/* We look at the difference once again. */
		float angular = (targetRotation - speed) / timeToTarget;

		/* Let's see if we exceed max acc. */
		float angAcc = Mathf.Abs (angular);
		if (angAcc > maxAngAcc) {
			angular /= angAcc; // This makes angular 1 or -1
			angular *= maxAngAcc;
		}
		Debug.Log (target + " " + a);
		speed += angular;
		transform.rotation = Quaternion.Euler (0, a + speed * Time.deltaTime, 0);

	}
}
