using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerBehavior : MonoBehaviour {
	[SerializeField] private Transform _area;
	[SerializeField] private GameObject _player;
	[SerializeField] private GameObject _camera;
	[SerializeField] private float _hologramScale;
	[SerializeField] private float _yPos;

	void Update () {
		setPosition();
		setRotation();
	}

	private void setPosition() {
		//Player's current location
		Vector3 _playerLocation = _player.transform.position;

		//Position relative to player and area
		Vector3 playerPos = _playerLocation - _area.position;

		//Minimap's position
		Vector3 parentPos = transform.parent.position;

		//Approximated position
		Vector3 newPos = parentPos + (playerPos * _hologramScale);
		newPos = new Vector3(newPos.x, parentPos.y + _yPos, newPos.z);

		transform.position = newPos;
	}

	private void setRotation() {
		transform.rotation = _camera.transform.rotation;
	}
}
