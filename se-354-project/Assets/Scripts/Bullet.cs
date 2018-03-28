using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public AITankScript parent;
	public Vector3 direction;
	public int damage;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += direction * 20 * Time.deltaTime;
	}
	
	void OnTriggerEnter(Collider other) {
		Destroy(gameObject);
		if(other.gameObject.layer == 10) {
			AITankScript enemy = other.GetComponent<AITankScript>();
			parent.increasePoints(enemy.takeAHit(damage));
		}
	}
}
