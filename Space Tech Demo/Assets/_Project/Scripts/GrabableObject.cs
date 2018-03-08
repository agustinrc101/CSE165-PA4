using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableObject : MonoBehaviour {
	[SerializeField] private FixedJoint _joint = null;
	[SerializeField] private GameObject _selectionObject;

	private bool hasJoint;

	// Use this for initialization
	void Start () {
		hasJoint = (_joint != null);
	}

	public void setActiveSelectionObject(bool b) { _selectionObject.SetActive(b); }
	
	public bool getHasJoint() { return hasJoint; }
}
