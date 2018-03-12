using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyBehavior : MonoBehaviour {
	[SerializeField] private GameObject _CenterEyeAnchor;
	
	private float _yOffset;
	private float _zOffset = 0;

	void Start() {
		_yOffset = transform.position.y;
		//_zOffset = transform.position.z;
	}

	// Update is called once per frame
	void Update () {
		followCameraPos();
	}

	private void followCameraPos() {
		Vector3 newPos = _CenterEyeAnchor.transform.position;
		newPos.y -= _yOffset;
		newPos.z -= _zOffset;
		transform.position = newPos;
	}
}
