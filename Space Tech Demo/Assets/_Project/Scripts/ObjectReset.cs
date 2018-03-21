using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReset : MonoBehaviour {
	[SerializeField] private float _secondsToReset = 5f;

	public bool isInBelt = false;
	private Vector3 initialPos;
	private Quaternion initialRot;
	private float time = 0f;

	private void Start() {
		initialPos = transform.position;
		initialRot = transform.rotation;
	}

	// Update is called once per frame
	void LateUpdate () {
		if (!(GetComponent<OVRGrabbable>().isGrabbed || isInBelt)) {
			time += Time.deltaTime;
			if (time >= _secondsToReset) {
				transform.position = initialPos;
				transform.rotation = initialRot;
			}
		}
		else {
			time = 0f;
		}
	}
}
