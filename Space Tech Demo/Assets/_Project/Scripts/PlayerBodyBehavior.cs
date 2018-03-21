using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyBehavior : MonoBehaviour {
	[SerializeField] private GameObject _CenterEyeAnchor;
	
	private float _yOffset = 0;
	private float _totalOffset = 0;

	void Start() {
		_yOffset = transform.position.y;
		_totalOffset = _yOffset;
	}

	// Update is called once per frame
	void Update () {
		followCameraPos();
	}

	private void followCameraPos() {
		Vector3 newPos = _CenterEyeAnchor.transform.position;
		newPos.y -= _totalOffset;
		transform.position = newPos;
		transform.rotation = Quaternion.Euler(0, _CenterEyeAnchor.transform.rotation.eulerAngles.y, 0);
	}

	public void setHeight(float o) {
		_totalOffset = 2f - o;
	}

	public float getHeight() { return transform.position.y; }
}
