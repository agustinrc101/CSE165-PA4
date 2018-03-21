using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPadCodeBehavior : MonoBehaviour {
	[SerializeField] private int[] _keyPadCodes = { 1234, 5834, 9862 };
	[SerializeField] private SpriteRenderer[] _digits;
	[SerializeField] private Sprite[] _sprites;
	[SerializeField] private MeshRenderer _display;
	[SerializeField] private Material _defaultMaterial;
	[SerializeField] private Material _success;
	[SerializeField] private Material _fail;
	[SerializeField] private AudioClip _successClip;
	[SerializeField] private AudioClip _failClip;
	[SerializeField] private KeyPadConnectionsBehavior[] _connections;

	private AudioSource audioSource;

	private int curDigit = 0;
	private int curCode = 0;

	void Start() {
		audioSource = GetComponent<AudioSource>();
		if (audioSource == null)
			Debug.LogError(name + " does not have an audio source");
	}

	public void inputDigit(int digit) {
		//Reset keys
		if (digit == 10) {
			reset();
		}
		else if (digit == 11) {
			enterCode();
		}
		else if (curDigit < 4 && curDigit >= 0) {
			enterDigit(digit);
		}
	}

	private void enterDigit(int digit) {
		_digits[curDigit].sprite = _sprites[digit];

		switch (curDigit) {
			case 0:
				curCode += 1000 * digit;
				break;
			case 1:
				curCode += 100 * digit;
				break;
			case 2:
				curCode += 10 * digit;
				break;
			case 3:
				curCode += digit;
				break;
			default:
				break;
		}
		curDigit++;
	}

	private void enterCode() {
		if (curDigit == 4) {
			for (int i = 0; i < _keyPadCodes.Length; i++) {
				if (curCode == _keyPadCodes[i]) {
					StartCoroutine(success());
					if(i < _connections.Length)
						_connections[i].activate();
					return;
				}
			}
		}
		GameObject.FindGameObjectWithTag("Robot").GetComponent<RobotBehavior>().codes(curCode);
		StartCoroutine(failure());
	}

	private IEnumerator success() {
		audioSource.clip = _successClip;
		audioSource.Play();
		_display.material = _success;
		yield return new WaitForSeconds(2.0f);
		_display.material = _defaultMaterial;
		reset();
	}

	private IEnumerator failure() {
		audioSource.clip = _failClip;
		audioSource.Play();
		_display.material = _fail;
		yield return new WaitForSeconds(0.4f);
		_display.material = _defaultMaterial;
		yield return new WaitForSeconds(0.4f);
		_display.material = _fail;
		yield return new WaitForSeconds(0.4f);
		_display.material = _defaultMaterial;
		yield return new WaitForSeconds(0.4f);
		_display.material = _fail;
		yield return new WaitForSeconds(0.4f);
		_display.material = _defaultMaterial;
		audioSource.Stop();
		reset();
	}

	private void reset() {
		curDigit = 0;
		curCode = 0;
		_digits[0].sprite = null;
		_digits[1].sprite = null;
		_digits[2].sprite = null;
		_digits[3].sprite = null;
	}

	
}
