using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadNumberBehavior : MonoBehaviour {
	[Range(0, 11)]
	[SerializeField] private int _digit;
	[SerializeField] private KeyPadCodeBehavior _display;
	[SerializeField] private float _pushDistance = 0.7158f;

	private bool press = false;
	private bool goingDown = true;
	private float startY, endY;

	void Start() {
		startY = transform.localPosition.y;
		endY = startY - _pushDistance;
	}

	void Update() {
		if (press) {
			if (goingDown) {
				transform.Translate(-transform.up * Time.deltaTime * 0.1f);
				if (transform.localPosition.y <= endY) {
					goingDown = false;
					transform.localPosition.Set(transform.localPosition.x, endY, transform.localPosition.z);
				}
			}
			else {
				transform.Translate(transform.up * Time.deltaTime * 0.1f);
				if (transform.localPosition.y >= startY) {
					goingDown = true;
					press = false;
					transform.localPosition.Set(transform.localPosition.x, startY, transform.localPosition.z);
				}
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.CompareTag("IndexFinger")) {
			press = true;
			GetComponent<AudioSource>().Play();
			_display.inputDigit(_digit);

			StartCoroutine(pushCooldown(other));
		}
	}

	private IEnumerator pushCooldown(Collider finger) {
		finger.gameObject.GetComponent<PointingDetector>().enabled = false;
		finger.enabled = false;
		yield return new WaitForSeconds(0.5f);
		finger.gameObject.GetComponent<PointingDetector>().enabled = true;
	}

}
