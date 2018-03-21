using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeLocalPosTEST : MonoBehaviour {
	[SerializeField] private Transform _purple;


	// Use this for initialization
	void Start () {
		Vector3 relPos = transform.InverseTransformPoint(_purple.position);
		Debug.Log(_purple.position);
		Debug.Log(relPos);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
