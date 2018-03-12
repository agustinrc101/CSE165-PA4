using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehavior : MonoBehaviour {
	[SerializeField] private Transform _centerEye;
	[Tooltip("This portal")]
	[SerializeField] private Transform _portal;
	[Tooltip("The connected Portal")]
	[SerializeField] private Transform _otherPortal;
	[SerializeField] private Camera _thisCam;
	[SerializeField] private Material _thisMat;

	void Start() {
		if (_thisCam.targetTexture != null)
			_thisCam.targetTexture.Release();

		int h = _centerEye.gameObject.GetComponent<Camera>().pixelHeight;
		int w = _centerEye.gameObject.GetComponent<Camera>().pixelWidth;

		_thisCam.targetTexture = new RenderTexture(w/2, h/2, 24);

		_thisMat.mainTexture = _thisCam.targetTexture;
	}

	// Update is called once per frame
	void LateUpdate () {
		Vector3 playerOffset = _centerEye.position - _otherPortal.position;
		transform.position = _portal.position + playerOffset;

		//Angular difference between portal rotations
		float angularDifference = Quaternion.Angle(_portal.rotation, _otherPortal.rotation);
		//Portal rotational difference
		Quaternion rotationalDiff = Quaternion.AngleAxis(angularDifference, Vector3.up);

		//Set camera direction
		Vector3 newCamDir = rotationalDiff * _centerEye.forward;
		transform.rotation = Quaternion.LookRotation(newCamDir, Vector3.up);
		Vector3 euler = transform.rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(euler.x, 0, 0);
	}
}
