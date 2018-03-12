using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationPlayer : MonoBehaviour {
	[SerializeField] private TeleportationArc _tpArc;
	[SerializeField] private GameObject _leftController;
	[SerializeField] private GameObject _rightController;
	[SerializeField] private OvrAvatar _avatar;
	[SerializeField] private LayerMask _layerMask;

	private bool _teleporting = false;
	private Vector3 _lastLocation;
	private Quaternion _lastRotation;
	private float _curYRotation;
	private float _lastYRotation;
	private int _curHand;

	private bool leftIsHolding = false;
	private bool rightIsHolding = false;

	// Use this for initialization
	void Start() {
		_lastLocation = new Vector3(0, transform.position.y, 0);
		_lastRotation = Quaternion.identity;
		_lastYRotation = 0f;
		_curYRotation = 0f;
	}

	// Update is called once per frame
	void Update() {
		if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick) != Vector2.zero && !leftIsHolding) {
			spawnArc((int)OvrAvatar.HandType.Left, OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick));
		}
		else if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick) != Vector2.zero && !rightIsHolding) {
			spawnArc((int)OvrAvatar.HandType.Right, OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick));
		}
		else if (_teleporting) {
			teleport();
		}
		else {
			_tpArc.gameObject.SetActive(false);
		}
	}

	private void spawnArc(int hand, Vector2 stick) {
		_tpArc.gameObject.SetActive(true);
		Transform t = this.transform;


		if (hand == 1) {
			t = _leftController.transform;
			_tpArc.gameObject.transform.position = _leftController.transform.position;
		}
		else {
			t = _rightController.transform;
			_tpArc.gameObject.transform.position = _rightController.transform.position;
		}

		_tpArc.showTPArc(t);
		setVars(hand, stick);
	}

	private void setPosition(Vector2 stick, OvrAvatar.HandType handType) {

	}

	private void teleport() {
		transform.position = _lastLocation;
		transform.rotation = _lastRotation;
		_lastYRotation = _curYRotation;
		_teleporting = false;
	}

	private void setVars(int hand, Vector2 stick) {
		_curHand = hand;
		_curYRotation = Mathf.Atan2(stick.x, stick.y) * 180 / Mathf.PI;
		_curYRotation += _lastYRotation;
		_lastRotation = Quaternion.Euler(0f, _curYRotation, 0f);
		_tpArc.setTPIndicatorRotation(_lastRotation);
	}

	public void canTeleport(bool b) {
		_teleporting = b;
	}

	public void setTPLocation(Vector3 pos) {
		if (pos.y != -100)
			_lastLocation = pos;
	}
}
