using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollisionTEST : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Debug.Log("Felt collision");
	}
}
