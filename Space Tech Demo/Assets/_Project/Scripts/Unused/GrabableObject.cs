using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour {
	[SerializeField] private bool _allowOffHandGrab = true;
	[SerializeField] private bool _snapPosition = false;
	[SerializeField] private bool _snapOrientation = false;
	[SerializeField] private Transform _snapOffset;
	[SerializeField] private Collider[] _grabPoints = null;
	[SerializeField] private GameObject _selectionObject;

	private bool isGrabbed = false;
	private bool grabbedKinematic = false;
	private Collider grabbedCollider = null;
	private GrabObject grabbedBy = null;
	private Transform par = null;

	void Awake() {
		if (_grabPoints.Length == 0) {
			Collider collider = GetComponent<Collider>();
			if (collider == null)
				Debug.LogError(name + " does not have a collider attached");
			_grabPoints = new Collider[1] { collider };
		}
	}

	void Start() {
		grabbedKinematic = GetComponent<Rigidbody>().isKinematic;
		par = transform.parent;
	}

	void OnDestroy() {
		if (grabbedBy != null)
			grabbedBy.forceRelease(this);
	}

	public void grabBegin(GrabObject hand, Collider grabPoint) {
		grabbedBy = hand;
		grabbedCollider = grabPoint;
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}

	public void grabEnd(Vector3 linearV, Vector3 angularV) {
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();
		rb.isKinematic = grabbedKinematic;
		rb.velocity = linearV;
		rb.angularVelocity = angularV;
		grabbedBy = null;
		grabbedCollider = null;
	}

	public void setActiveSelectionObject(bool b) { _selectionObject.SetActive(b); }

	//Getters
	public bool getIsGrabbed() { return isGrabbed; }
	public bool getAllowOffHandGrab() { return _allowOffHandGrab; }
	public bool getSnapPosition() { return _snapPosition; }
	public bool getSnapOrientation() { return _snapOrientation; }
	public Transform getSnapOffset() { return _snapOffset; }
	public Transform getGrabbedTransform() { return grabbedCollider.transform; }
	public Rigidbody getGrabbedRigidbody() {
		if (grabbedCollider == null) grabbedCollider = GetComponent<Collider>();
		return grabbedCollider.attachedRigidbody;
	}
	public Collider[] getGrabPoints() { return _grabPoints; }
	public GrabObject getGrabbedBy() {return grabbedBy; }
	public Transform getParent() { return par; }
}
