using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMBehavior : MonoBehaviour {
	public enum Locations {
		HUB, DESERT, FOREST, RANGE
	}

	[SerializeField] private AudioClip[] _songs;

	private GameObject _player;
	private GameObject _minimap;
	private Locations _curLocation = Locations.HUB;
	private AudioSource _audio;

	// Use this for initialization
	void Start () {
		_player = GameObject.FindGameObjectWithTag("Player");
		_minimap = GameObject.FindGameObjectWithTag("Minimap");
		_audio = GetComponent<AudioSource>();
		_audio.clip = _songs[0];
		_audio.Play();
	}

	public void setCurrentArea(Locations loc, float time) {
		areaSetup(loc);
		audioSetup(time);
	}

	private void areaSetup(Locations loc) {
		_curLocation = loc;
		_minimap.GetComponent<MinimapBehavior>().switchMap((int)loc);
	}

	private void audioSetup(float time) {
		_audio.Stop();
		_audio.clip = _songs[(int)_curLocation];
		_audio.time = time;
		_audio.Play();
	}
}
