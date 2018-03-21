using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour {
	[SerializeField] private float _grabBegin = 0.55f;
	[SerializeField] private float _grabEnd = 0.35f;
	[SerializeField] private bool _parentHeldObject = true;
	[Tooltip("Where to hold objects")]
	[SerializeField] private Transform _handTransform = null;
	[SerializeField] private Collider[] _grabVolumes = null;
	[SerializeField] private OVRInput.Controller _controller;
	[SerializeField] private Transform _parentTransform;
	[SerializeField] private AudioClip _rumbleHz;

	private Dictionary<GrabableObject, int> grabCandidates = new Dictionary<GrabableObject, int>();
	private GrabableObject grabbedObj;
	private Vector3 lastPos;
	private Vector3 offsetPosition;
	private Vector3 grabbedObjPos;
	private Quaternion lastRot;
	private Quaternion offsetRotation;
	private Quaternion grabbedObjRot;
	private float prevFlex;
	private bool noCameraRig = true;
	private bool grabVolumeEnabled = true;

	void Awake() {
		offsetPosition = transform.localPosition;
		offsetRotation = transform.localRotation;

		OVRCameraRig rig = null;
		if (transform.parent != null && transform.parent.parent != null)
			rig = transform.parent.parent.GetComponent<OVRCameraRig>();

		if (rig != null) {
			rig.UpdatedAnchors += (r) => { OnUpdateAnchors(); };
			noCameraRig = false;
		}
	}

	void Start() {
		lastPos = transform.position;
		lastRot = transform.rotation;
		if (_parentTransform == null) {
			if (gameObject.transform.parent != null)
				_parentTransform = gameObject.transform.parent.transform;
			else {
				_parentTransform = new GameObject().transform;
				_parentTransform.position = Vector3.zero;
				_parentTransform.rotation = Quaternion.identity;
			}
		}
	}

	void FixedUpdate() {
		if (noCameraRig)
			OnUpdateAnchors();
	}

	void OnDestroy() {
		if (grabbedObj != null)
			grabEnd();
	}

	void OnUpdateAnchors() {
		Vector3 handPos = OVRInput.GetLocalControllerPosition(_controller);
		Quaternion handRot = OVRInput.GetLocalControllerRotation(_controller);
		Vector3 destPos = _parentTransform.TransformPoint(offsetPosition + handPos);
		Quaternion destRot = _parentTransform.rotation * handRot * offsetRotation;
		GetComponent<Rigidbody>().MovePosition(destPos);
		GetComponent<Rigidbody>().MoveRotation(destRot);

		if (_parentHeldObject)
			moveGrabbedObject(destPos, destRot);

		lastPos = transform.position;
		lastRot = transform.rotation;

		float lastFlex = prevFlex;
		prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller);
		checkForGrabOrRelease(lastFlex);
	}

	void OnTriggerEnter(Collider other) {
		GrabableObject grabbable = other.GetComponent<GrabableObject>() ?? other.GetComponentInParent<GrabableObject>();
		if (grabbable == null) return;
		if (!other.gameObject.GetComponent<GrabableObject>().getIsGrabbed()) {
			highlight(true, grabbable);
			rumble();
		}

		int refCount = 0;
		grabCandidates.TryGetValue(grabbable, out refCount);
		grabCandidates[grabbable] = refCount + 1;
	}

	void OnTriggerExit(Collider other) {
		GrabableObject grabbable = other.GetComponent<GrabableObject>() ?? other.GetComponentInParent<GrabableObject>();
		if (grabbable == null) return;

		highlight(false, grabbable);

		int refCount = 0;
		bool found = grabCandidates.TryGetValue(grabbable, out refCount);
		if (!found) return;

		if (refCount > 1)
			grabCandidates[grabbable] = refCount - 1;
		else
			grabCandidates.Remove(grabbable);
	}

	private void checkForGrabOrRelease(float lastFlex) {
		if (prevFlex >= _grabBegin && lastFlex < _grabBegin)
			grabBegin();
		else if (prevFlex <= _grabEnd && lastFlex > _grabEnd)
			grabEnd();
	}

	private void grabBegin() {
		float closestMagSq = float.MaxValue;
		GrabableObject closestGrabable = null;
		Collider closestGrabbableCollider = null;

		foreach (GrabableObject grabbable in grabCandidates.Keys) {
			bool canGrab = !(grabbable.getIsGrabbed() && !grabbable.getAllowOffHandGrab());

			if (!canGrab) continue;

			for (int j = 0; j < grabbable.getGrabPoints().Length; j++) {
				Collider grabbableCollider = grabbable.getGrabPoints()[j];
				Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(_handTransform.position);
				float grabbableMagSq = (_handTransform.position - closestPointOnBounds).sqrMagnitude;
				if (grabbableMagSq < closestMagSq) {
					closestGrabable = grabbable;
					closestGrabbableCollider = grabbableCollider;
				}
			}
		}

		grabVolumeEnable(false);

		if (closestGrabable != null) {
			if (closestGrabable.getIsGrabbed())
				closestGrabable.getGrabbedBy().offHandGrabbed(closestGrabable);

			grabbedObj = closestGrabable;
			grabbedObj.grabBegin(this, closestGrabbableCollider);
			highlight(false, grabbedObj);

			lastPos = transform.position;
			lastRot = transform.rotation;

			if (grabbedObj.getSnapPosition()) {
				grabbedObjPos = _handTransform.localPosition;
				if (grabbedObj.getSnapOffset()) {
					Vector3 snapOffset = grabbedObj.getSnapOffset().position;
					if (_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
					grabbedObjPos += snapOffset;
				}
			}
			else {
				Vector3 relPos = grabbedObj.transform.position - transform.position;
				relPos = Quaternion.Inverse(transform.rotation) * relPos;
				grabbedObjPos = relPos;
			}

			if (grabbedObj.getSnapOrientation()) {
				grabbedObjRot = _handTransform.localRotation;
				if (grabbedObj.getSnapOffset()) grabbedObjRot = grabbedObj.getSnapOffset().rotation * grabbedObjRot;
			}
			else {
				Quaternion relOri = Quaternion.Inverse(transform.rotation) * grabbedObj.transform.rotation;
				grabbedObjRot = relOri;
			}

			moveGrabbedObject(lastPos, lastRot, true);
			if (_parentHeldObject) grabbedObj.transform.parent = transform;
		}
	}

	private void moveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false) {
		if (grabbedObj == null) return;

		Rigidbody grabbedRB = grabbedObj.getGrabbedRigidbody();
		Vector3 grabbablePos = pos + rot * grabbedObjPos;
		Quaternion grabbableRot = rot * grabbedObjRot;

		if (forceTeleport) {
			grabbedRB.transform.position = grabbablePos;
			grabbedRB.transform.rotation = grabbableRot;
		}
		else {
			grabbedRB.MovePosition(grabbablePos);
			grabbedRB.MoveRotation(grabbableRot);
		}
	}

	private void grabEnd() {
		if (grabbedObj != null) {
			OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(_controller), orientation = OVRInput.GetLocalControllerRotation(_controller) };
			OVRPose offsetPose = new OVRPose { position = offsetPosition, orientation = offsetRotation };
			localPose = localPose * offsetPose;

			OVRPose trackingspace = transform.ToOVRPose() * localPose.Inverse();
			Vector3 linearVelocity = trackingspace.orientation * OVRInput.GetLocalControllerVelocity(_controller);
			Vector3 angularVelocty = trackingspace.orientation * OVRInput.GetLocalControllerAngularVelocity(_controller);

			grabbableRelease(linearVelocity, angularVelocty);
		}

		grabVolumeEnable(true);
	}

	private void grabbableRelease(Vector3 linearV, Vector3 angularV) {
		grabbedObj.grabEnd(linearV, angularV);
		if (_parentHeldObject) grabbedObj.transform.parent = grabbedObj.getParent();
		grabbedObj = null;
	}

	private void grabVolumeEnable(bool enabled) {
		if (grabVolumeEnabled == enabled) return;

		grabVolumeEnabled = enabled;
		for (int i = 0; i < _grabVolumes.Length; i++)
			_grabVolumes[i].enabled = grabVolumeEnabled;

		if (!grabVolumeEnabled)
			grabCandidates.Clear();
	}

	private void offHandGrabbed(GrabableObject grabbable) {
		if (grabbedObj == grabbable)
			grabbableRelease(Vector3.zero, Vector3.zero);
	}

	private void rumble() {
		OVRHaptics.Channels[(int)_controller - 1].Preempt(new OVRHapticsClip(_rumbleHz));
		OVRHaptics.Process();
	}

	private void highlight(bool b, GrabableObject grabbable) {
		if (grabbable == null) return;
		grabbable.setActiveSelectionObject(b);
	}

	public GrabableObject getGrabbedObject() { return grabbedObj; }

	public void forceRelease(GrabableObject grabable) {
		if (grabbedObj != null && grabbedObj == grabable)
			grabEnd();
	}


}
