using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleportationBehavior : MonoBehaviour {
	[SerializeField] private Transform _player;
	[SerializeField] private Transform _centerEye;
	[SerializeField] private Transform _receiver;
	[SerializeField] private GMBehavior.Locations _destination;
	[SerializeField] private AudioSource _planeAudioSource;

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Toolbelt"))
			teleportPlayer();
	}

	private void teleportPlayer() {
		Vector3 relativePos = transform.InverseTransformPoint(_centerEye.position);

		//Teleport moves across portal
		if (relativePos.y - transform.localPosition.y > 0f) {
			Vector3 dir = _player.position - transform.position;
			float rotationDiff = Quaternion.Angle(transform.rotation, _receiver.rotation);
			rotationDiff += 180;
			_player.Rotate(Vector3.up, rotationDiff);

			Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * dir;
			_player.position = _receiver.position + positionOffset;

			float time = _planeAudioSource.time;
			GameObject.FindGameObjectWithTag("GM").GetComponent<GMBehavior>().setCurrentArea(_destination, time);
		}
	}

}
