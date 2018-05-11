using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface State{
	void Enter (Tank tnk);
	void Execute (Tank tnk);
	void Exit (Tank tnk);
}

public class StateTank : MonoBehaviour {

	public GameObject tank;
	void Start(){
		tank = GameObject.FindGameObjectWithTag ("player");
	}

}
