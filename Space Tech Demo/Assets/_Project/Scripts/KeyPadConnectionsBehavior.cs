using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadConnectionsBehavior : MonoBehaviour {
	[SerializeField] private AudioClip _endingSound;
	[SerializeField] private float _yPosition;
	[SerializeField] private bool _disableCollider = false;
	private bool unlock = false;
	private bool unlockOnce = true;
	
	// Update is called once per frame
	void Update () {
		if (unlock && unlockOnce) {
			transform.Translate(Vector3.down * Time.deltaTime);
			if (transform.position.y <= _yPosition)
				opened();
		}
	}

	private void opened() {
		unlockOnce = unlock = false;
		GetComponent<AudioSource>().Stop();
		GetComponent<AudioSource>().clip = _endingSound;
		GetComponent<AudioSource>().volume = 1f;
		GetComponent<AudioSource>().loop = false;
		GetComponent<AudioSource>().Play();
		if (GetComponent<Collider>() != null)
			GetComponent<Collider>().enabled = !_disableCollider;
		transform.position = new Vector3(transform.position.x, _yPosition, transform.position.z);
	}

	public void activate() {
		unlock = true;
		if (unlockOnce) {
			GetComponent<AudioSource>().loop = true;
			GetComponent<AudioSource>().volume = 0.6f;
		}
		else
			GetComponent<AudioSource>().volume = 1f;

		GetComponent<AudioSource>().Play();

	}
}
