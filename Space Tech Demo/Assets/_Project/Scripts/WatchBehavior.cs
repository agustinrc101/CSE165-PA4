using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchBehavior : MonoBehaviour {
	[SerializeField] private Canvas _watchCanvas;
	[SerializeField] private bool _uiEnabled = false;

	private float _buttonAnimationTime = 1.5f;
	private float _time = 0.0f;

	// Use this for initialization
	void Start() {
		if (_watchCanvas == null)
			Debug.LogWarning("Watch does not have a canvas attached");
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("IndexFinger")) {
			_uiEnabled = !_uiEnabled;
			_watchCanvas.gameObject.SetActive(_uiEnabled);
			GetComponent<AudioSource>().Play();
		}
	}
}
