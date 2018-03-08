using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ideas from http://www.rgbschemes.com/blog/oculus-touch-and-finger-stuff-part-1/
public class HandsBehavior : MonoBehaviour {
	public enum HandState {
		EMPTY, TOUCHING, HOLDING
	};

	[SerializeField] private OVRInput.Controller _controller;
	[SerializeField] private Rigidbody _attachPoint = null;
	[SerializeField] private string _grabableTag = "isGrabable";
	[SerializeField] private GameObject _player;
	[SerializeField] private AudioClip _rumbleHz;

	private HandState _handState = HandState.EMPTY;
	private Rigidbody _heldObject;
	private FixedJoint _tempJoint;
	
	// Update is called once per frame
	void Update () {
		switch (_handState) {
			case HandState.TOUCHING:
				if (_tempJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) >= 0.5f) {
					_heldObject.velocity = Vector3.zero;

					_tempJoint = _heldObject.gameObject.GetComponent<FixedJoint>();
					if (_tempJoint == null)
						_tempJoint = _heldObject.gameObject.AddComponent<FixedJoint>();

					_tempJoint.connectedBody = _attachPoint;
					_handState = HandState.HOLDING;
					highlight(false);
				}

				break;
			case HandState.HOLDING:
				if (_tempJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) < 0.5f) {
					if (!_heldObject.gameObject.GetComponent<GrabableObject>().getHasJoint()) {
						if (_tempJoint != null)
							Object.DestroyImmediate(_tempJoint);
					}

					_tempJoint = null;
					throwObject();
					_handState = HandState.EMPTY;
				}
				else if (_tempJoint == null)
					_handState = HandState.EMPTY;

				break;
			case HandState.EMPTY:
				highlight(false);
				break;
			default:
				break;
		}

		if (_controller == OVRInput.Controller.RTouch && _handState == HandState.HOLDING)
			Debug.Log(_tempJoint != null);
	}

	private void throwObject() {
		Quaternion playerRot = _player.transform.rotation;
		_heldObject.velocity = OVRInput.GetLocalControllerVelocity(_controller);
		_heldObject.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(_controller);
		_heldObject.maxAngularVelocity = _heldObject.angularVelocity.magnitude;
	}

	//Touched
	void OnTriggerEnter(Collider other) {
		triggerDetection(other);
		if (other.CompareTag(_grabableTag))
			rumble();
	}
	
	//Touching
	void OnTriggerStay(Collider other) {
		triggerDetection(other);
	}

	private void triggerDetection(Collider other) {
		if (other.CompareTag(_grabableTag) && _handState == HandState.EMPTY
			&& OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) < 0.5f) {

			GameObject temp = other.gameObject;
			if (temp != null) {
				Rigidbody rb = temp.GetComponent<Rigidbody>();
				if (rb == null) {
					Debug.LogWarning(other.name + " does not have a rigidbody");
					return;
				}

				highlight(false);

				_heldObject = rb;
				_handState = HandState.TOUCHING;
				highlight(true);
			}
		}
	}

	private void rumble() {
		OVRHaptics.Channels[(int)_controller - 1].Preempt(new OVRHapticsClip(_rumbleHz));
		OVRHaptics.Process();
	}
	//Not Touching
	void OnTriggerExit(Collider other) {
		if (other.CompareTag(_grabableTag) && _handState != HandState.HOLDING) {
			highlight(false);
			_heldObject = null;
			_handState = HandState.EMPTY;
		}
		
	}

	private void highlight(bool b) {
		if (_heldObject == null)
			return;

		_heldObject.gameObject.GetComponent<GrabableObject>().setActiveSelectionObject(b);
	}
}
