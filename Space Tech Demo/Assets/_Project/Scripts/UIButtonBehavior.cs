using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonBehavior : MonoBehaviour {
	private enum SettingsType {
		Volume, BeltHeight, PlayerSpeed, RotateLeft, RotateRight
	}

	[SerializeField] private SettingsType _setting;
	[SerializeField] private Transform _leftBound = null;
	[SerializeField] private Transform _rightBound = null;
	[SerializeField] private Material _pushMaterial;

	private Transform _objectOfInterest;
	private bool isSlider = false;
	private Material mat;

	void OnEnable() {
		if (mat == null)
			mat = GetComponent<MeshRenderer>().material;
		GetComponent<MeshRenderer>().material = mat;
		GetComponent<Collider>().enabled = true;
	}

	void Start() {
		if (_setting == SettingsType.BeltHeight) {
			_objectOfInterest = GameObject.FindGameObjectWithTag("Toolbelt").transform;
			isSlider = true;
		}
		else if (_setting == SettingsType.Volume) {
			isSlider = true;
			float distance = _rightBound.localPosition.x - _leftBound.localPosition.x;
			distance *= AudioListener.volume;
			transform.position = new Vector3(_leftBound.localPosition.x + distance, transform.localPosition.y, transform.localPosition.z);
		}
		else if (_setting == SettingsType.PlayerSpeed) {
			_objectOfInterest = GameObject.FindGameObjectWithTag("Player").transform;

			isSlider = true;
			float distance = _rightBound.localPosition.x - _leftBound.localPosition.x;
			float curSpeedPos = (_objectOfInterest.GetComponent<TravelLocomotion>().getSpeed() - 1.5f)/2f;
			curSpeedPos = _leftBound.localPosition.x + (distance * curSpeedPos);
			transform.localPosition = new Vector3(curSpeedPos, transform.localPosition.y, transform.localPosition.z);
		}
		else {
			_objectOfInterest = GameObject.FindGameObjectWithTag("Player").transform;
			isSlider = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (_setting == SettingsType.Volume) {
			if (transform.localPosition.x < _leftBound.localPosition.x)
				transform.position = _leftBound.position;
			if (transform.localPosition.x > _rightBound.localPosition.x)
				transform.position = _rightBound.position;

			float vol = transform.localPosition.x - _leftBound.localPosition.x;
			vol /= (_rightBound.localPosition.x - _leftBound.localPosition.x);

			AudioListener.volume = vol;
			transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
		}
		else if (_setting == SettingsType.BeltHeight) {
			if (transform.localPosition.x < _leftBound.localPosition.x)
				transform.position = _leftBound.position;
			if (transform.localPosition.x > _rightBound.localPosition.x)
				transform.position = _rightBound.position;

			float difference = transform.localPosition.x - _leftBound.localPosition.x;
			difference /= (_rightBound.localPosition.x - _leftBound.localPosition.x);
			_objectOfInterest.GetComponent<PlayerBodyBehavior>().setHeight(difference);
			transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
		}
		else if (_setting == SettingsType.PlayerSpeed) {
			if (transform.localPosition.x < _leftBound.localPosition.x)
				transform.position = _leftBound.position;
			if (transform.localPosition.x > _rightBound.localPosition.x)
				transform.position = _rightBound.position;

			float difference = transform.localPosition.x - _leftBound.localPosition.x;
			difference /= (_rightBound.localPosition.x - _leftBound.localPosition.y);
			_objectOfInterest.GetComponent<TravelLocomotion>().setSpeed(difference);
			transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
		}
	}

	private void settingsDelegator() {
		switch (_setting) {
			case SettingsType.RotateLeft:
				_objectOfInterest.Rotate(0, -90f, 0);	//Rotate Player
				break;
			case SettingsType.RotateRight:
				_objectOfInterest.transform.Rotate(0, 90f, 0);	//Rotate Player
				break;
			default:
				break;
		}
	}

	void OnTriggerEnter(Collider other) {
		//Buttons
		if (other.CompareTag("IndexFinger")) {
			if (!isSlider) {
				settingsDelegator();
				StartCoroutine(cooldown());
			}
		}
		else
			Physics.IgnoreCollision(other.GetComponent<Collider>(), GetComponent<Collider>());
	}

	void OnTriggerStay(Collider other) {
		//Sliders
		if (other.CompareTag("IndexFinger")) {
			if (isSlider)
				slide(other.ClosestPoint(transform.position));
		}
	}
	
	private void slide(Vector3 pointOnFinger) {
		Vector3 dir = (pointOnFinger - transform.position).normalized;
		transform.Translate(Vector3.right * dir.x * 0.01f);
	}

	private IEnumerator cooldown() {
		Material m = GetComponent<MeshRenderer>().material;
		GetComponent<AudioSource>().Play();
		GetComponent<MeshRenderer>().material = _pushMaterial;
		GetComponent<Collider>().enabled = false;
		yield return new WaitForSeconds(0.5f);
		GetComponent<Collider>().enabled = true;
		GetComponent<MeshRenderer>().material = m;
	}
}
