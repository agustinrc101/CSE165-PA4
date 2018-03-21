using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsBasketBehavior : MonoBehaviour {
	[SerializeField] private Text _pointsDisplay;
	[SerializeField] private AudioSource _pointsScreen;
	[SerializeField] private RobotBehavior _robot;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("isGrabable")) {
			if (other.GetComponent<ObjectPoints>() != null && !other.GetComponent<ObjectReset>().isInBelt && other.gameObject != null) {
				int p = int.Parse(_pointsDisplay.text);
				p += other.GetComponent<ObjectPoints>().points;
				_pointsDisplay.text = "" + p;
				_pointsDisplay.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
				_pointsScreen.Play();
				_robot.updateScore(p);
				StartCoroutine(disableObject(other.gameObject));
			}
		}
	}

	private IEnumerator disableObject(GameObject obj) {
		yield return new WaitForSeconds(0.5f);
		GameObject.Destroy(obj);
	}
}
