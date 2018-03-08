using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleportation : MonoBehaviour {
	[SerializeField] private GameObject _teleportMarker;
	[SerializeField] private OvrAvatar _avatar;
	[SerializeField] private LayerMask _layerMask;

	private bool _teleporting = false;
	private Vector3 _lastLocation;
	private Quaternion _lastRotation;
	private float _curYRotation;
	private float _lastYRotation;

	private bool leftIsHolding = false;
	private bool rightIsHolding = false;

	void Start() {
		_lastLocation = new Vector3(0, transform.position.y, 0);
		_lastRotation = Quaternion.identity;
		_lastYRotation = 0f;
		_curYRotation = 0f;
	}

	// Update is called once per frame
	void Update() {
		if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != Vector2.zero && !leftIsHolding) {
			_teleportMarker.SetActive(true);
			_teleporting = true;

			Vector3 pos = _avatar.GetHandTransform(OvrAvatar.HandType.Left, OvrAvatar.HandJoint.HandBase).transform.position;
			Vector3 forward = Vector3.zero;
			Vector3 up = Vector3.zero;

			_avatar.GetPointingDirection(OvrAvatar.HandType.Left, ref forward, ref up);

			Ray ray = new Ray(pos, forward);
			RaycastHit hitInfo;

			if (Physics.Raycast(ray, out hitInfo, 50, _layerMask)) {
				Vector2 xy = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
				_curYRotation = Mathf.Atan2(xy.x, xy.y) * 180 / Mathf.PI;
				_curYRotation += _lastYRotation;
				_lastRotation = Quaternion.Euler(0f, _curYRotation, 0f);

				_teleportMarker.transform.rotation = _lastRotation;
				_teleportMarker.transform.position = new Vector3(hitInfo.point.x, _teleportMarker.transform.position.y, hitInfo.point.z);
				_lastLocation = new Vector3(hitInfo.point.x, _lastLocation.y, hitInfo.point.z);
			}
		}
		else if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != Vector2.zero && !rightIsHolding) {
			_teleportMarker.SetActive(true);
			_teleporting = true;

			Vector3 pos = _avatar.GetHandTransform(OvrAvatar.HandType.Right, OvrAvatar.HandJoint.HandBase).transform.position;
			Vector3 forward = Vector3.zero;
			Vector3 up = Vector3.zero;

			_avatar.GetPointingDirection(OvrAvatar.HandType.Right, ref forward, ref up);

			Ray ray = new Ray(pos, forward);
			RaycastHit hitInfo;

			if (Physics.Raycast(ray, out hitInfo, 50, _layerMask)) {
				Vector2 xy = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
				_curYRotation = Mathf.Atan2(xy.x, xy.y) * 180 / Mathf.PI;
				_curYRotation += _lastYRotation;
				_lastRotation = Quaternion.Euler(0f, _curYRotation, 0f);

				_teleportMarker.transform.rotation = _lastRotation;
				_teleportMarker.transform.position = new Vector3(hitInfo.point.x, _teleportMarker.transform.position.y, hitInfo.point.z);
				_lastLocation = new Vector3(hitInfo.point.x, _lastLocation.y, hitInfo.point.z);
			}
		}
		else {
			if (_teleporting) {
				transform.position = _lastLocation;
				transform.rotation = _lastRotation;
				_lastYRotation = _curYRotation;
				_teleportMarker.SetActive(false);
				_teleporting = false;
			}
		}
	}

	public void setLeftHold(bool b) { leftIsHolding = b;}
	public void setRightHold(bool b) { rightIsHolding = b;}
}
