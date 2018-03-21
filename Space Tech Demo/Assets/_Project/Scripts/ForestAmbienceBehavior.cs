using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestAmbienceBehavior : MonoBehaviour {
	[SerializeField] private float _minTime = 1f;
	[SerializeField] private float _maxTime = 5f;

	private AudioSource audioSource;
	private float maxTime = 0f;
	private float time = 0f;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!audioSource.isPlaying) {
			if (maxTime == 0)
				maxTime = Random.Range(_minTime, _maxTime);
			time += Time.deltaTime;
			if (time >= maxTime) {
				audioSource.Play();
				maxTime = time = 0f;
			}
		}
	}
}
