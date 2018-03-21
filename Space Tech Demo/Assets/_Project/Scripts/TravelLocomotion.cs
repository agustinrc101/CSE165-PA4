using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelLocomotion : MonoBehaviour {
	private enum Hand {
		Left, Right
	};

	[SerializeField] private GameObject _left;
	[SerializeField] private GameObject _right;
	[SerializeField] private GameObject _centerEye;
	[SerializeField] private float _speed = 2.5f;

	private float _time = 0f;
	private float _coolDown = 0f;
	private Vector3 _dir;

	private Hand _curMaxHand;
	private float _leftY;
	private float _rightY;

	private bool collided = false;
	private bool isGetData = true;
	
	// Update is called once per frame
	void Update () {
		if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger)) {
			_leftY = _left.transform.position.y;
			_rightY = _right.transform.position.y;

			_time += Time.deltaTime;
			if (_time > 0.3f && !collided) {
				begin(isGetData);
				delegator();
			}


		}
		else {
			_time = 0f;
			isGetData = true;
		}

		if (_coolDown > 0f) {
			_coolDown -= Time.deltaTime;
			transform.Translate(_speed * _dir * Time.deltaTime);
		}

		if (transform.position.y < -5f)
			transform.position = new Vector3(0, 0.5f, -5f);
	}

	private void begin(bool b) {
		if (b) {
			isGetData = false;
			if (_leftY > _rightY)
				_curMaxHand = Hand.Left;
			else
				_curMaxHand = Hand.Right;
		}

	}

	private void delegator() {
		if (_curMaxHand == Hand.Left)
			locomotion(_leftY, _rightY);
		else
			locomotion(_rightY, _leftY);
	}

	private void locomotion(float higher, float lower) {
		float diff = Mathf.Abs(higher - lower);
		//Debug.Log(diff);
		if (diff > 0.3f) {
			if (_curMaxHand == Hand.Left) {
				if (_leftY < _rightY)
					reachedMaxMin();
			}
			else {
				if (_rightY < _leftY)
					reachedMaxMin();
			}
		}
	}

	private void reachedMaxMin() {
		if (_curMaxHand == Hand.Left)
			_curMaxHand = Hand.Right;
		else
			_curMaxHand = Hand.Left;

		moving();
	}

	private void moving() {
		Vector3 p1 = new Vector3(_right.transform.localPosition.x, 0, _right.transform.localPosition.z);
		Vector3 p2 = new Vector3(_left.transform.localPosition.x, 0, _left.transform.localPosition.z);
		Vector3 p3 = new Vector3(_centerEye.transform.localPosition.x, 0, _centerEye.transform.localPosition.z);
		_dir = (((p1 + p2) / 2f) - p3).normalized;
		_coolDown = 0.3f;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer != 8) {
			Physics.IgnoreCollision(GetComponent<Collider>(), other);
		}
	}

	public void setSpeed(float s) {
		_speed = 1.5f + s;
	}

	public float getSpeed() { return _speed; }
}
