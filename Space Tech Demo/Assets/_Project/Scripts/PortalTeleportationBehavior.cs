using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleportationBehavior : MonoBehaviour {
	[SerializeField] private Transform _player;
	[SerializeField] private Transform _receiver;

	private bool playerOverlap = false;

	// Update is called once per frame
	void Update () {
		if (playerOverlap) {
			Vector3 dir = _player.position - transform.position;

			float dotProd = Vector3.Dot(transform.up, dir);
			//Teleport moves across portal
			if (dotProd < 0f) {
				float rotationDiff = Quaternion.Angle(transform.rotation, _receiver.rotation);
				rotationDiff += 180;
				_player.Rotate(Vector3.up, rotationDiff);

				Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * dir;
				_player.position = _receiver.position + positionOffset;

				playerOverlap = false;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Toolbelt"))
			playerOverlap = true;
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("Toolbelt"))
			playerOverlap = false;
	}
}
