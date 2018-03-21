using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerBehavior : MonoBehaviour {
	[SerializeField] private Transform _area;
	[SerializeField] private GameObject _centerEye;
	[SerializeField] private float _hologramScale;

	void FixedUpdate () {
		setPosition();
		setRotation();
	}

	private void setPosition() {
		//Player's current location
		Vector3 _playerLocation = _centerEye.transform.position;

		//Position relative to player and area (world coord)
		Vector3 playerPos = _playerLocation - _area.position;

		//Minimap's position (local area)
		Vector3 parentPos = transform.parent.localPosition;

		//Approximated position
		Vector3 newPos = parentPos + (playerPos * _hologramScale);
		transform.localPosition = newPos;
	}

	private void setRotation() {
		Quaternion rot = Quaternion.Euler(0, _centerEye.transform.rotation.eulerAngles.y, 0);
		transform.localRotation = rot;
	}
}
