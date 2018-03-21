using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbeltPouchBehavior : MonoBehaviour {

	private bool isCarrying = false;

	private Transform curHover = null;
	private bool curIsGrabbed = false;

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("isGrabable") && other.GetComponent<OVRGrabbable>().isGrabbed && !isCarrying) {
			curHover = other.transform;
			curIsGrabbed = other.gameObject.GetComponent<OVRGrabbable>().isGrabbed;
		}
	}

	void OnTriggerStay(Collider other) {
		if (other.CompareTag("isGrabable")) {
			if (curHover == other.transform && curIsGrabbed){
				if (!other.GetComponent<OVRGrabbable>().isGrabbed) {
					other.GetComponent<Rigidbody>().isKinematic = true;
					other.GetComponent<ObjectReset>().isInBelt = true;
					curHover.parent = this.transform;
					curHover.localPosition = Vector3.zero;

					if (curHover.GetComponent<ObjectPoints>().points == 2)
						curHover.localRotation = Quaternion.Euler(-90, 0, 0);
					else
						curHover.localRotation = Quaternion.identity;

					isCarrying = true;
				}
				else
					curHover.GetComponent<OVRGrabbable>().vibrateController();
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.CompareTag("isGrabable")){
			if (curHover == other.transform && curHover.GetComponent<OVRGrabbable>().isGrabbed) {
				other.GetComponent<ObjectReset>().isInBelt = false;
				isCarrying = false;
				curIsGrabbed = false;
				curHover = null;
			}
			else {
				if (curHover != null) {
					curHover.localPosition = Vector3.zero;

					if (curHover.GetComponent<ObjectPoints>().points == 2)
						curHover.localRotation = Quaternion.Euler(-90, 0, 0);
					else
						curHover.localRotation = Quaternion.identity;
				}
			}
		}
	}
}
