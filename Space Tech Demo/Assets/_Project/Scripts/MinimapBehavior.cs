using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBehavior : MonoBehaviour {
	[SerializeField] private GameObject[] _maps;
	[SerializeField] private GameObject _minimapOrb;
	[SerializeField] private bool _snapBackToBelt = true;
	[SerializeField] private Transform _pouch;

	private int curLocation = 0;
	private OVRGrabbable _gScript;

	private bool iterateOnce = false;

	void Start() {
		_gScript = GetComponent<OVRGrabbable>();
	}

	void Awake() {
		switchMap(curLocation);
		_gScript = GetComponent<OVRGrabbable>();
	}

	void Update() {
		if (!_gScript.isGrabbed) {
			if (!iterateOnce) {
				for (int i = 0; i < _maps.Length; i++)
					_maps[i].SetActive(false);
				_minimapOrb.SetActive(true);
				if (_snapBackToBelt) {
					transform.parent = _pouch;
					transform.localPosition = Vector3.zero;
				}

				iterateOnce = true;
			}
		}
		else {
			if (iterateOnce) {
				_maps[curLocation].SetActive(true);
				_minimapOrb.SetActive(false);
				iterateOnce = false;
			}
		}
	}

	public void enableMap(bool b) {
		this.gameObject.SetActive(b);
	}

	public void switchMap(int loc) {
		curLocation = loc;

		if (GetComponent<OVRGrabbable>().isGrabbed) {
			for (int i = 0; i < _maps.Length; i++) {
				_maps[i].SetActive(i == loc);
			}
		}
	}
}
