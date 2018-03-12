using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationArc : MonoBehaviour {
	[Tooltip("The larger the velocity, the longer the distance")]
	[SerializeField] private float _startVelocity = 7f;
	[SerializeField] private float _angle;
	[SerializeField] private float _gravity = -9.8f;
	[Tooltip("Determines how smooth the arc will be (higher numbers mean the arc will be smoother)")]
	[Range(5, 200)]
	[SerializeField] private int _resolution = 50;
	[SerializeField] private float _width = 0.2f;
	[SerializeField] private LayerMask _layerMask;
	[SerializeField] private GameObject _tpIndicator;
	[SerializeField] private GameObject _cantTpIndicator;
	[SerializeField] private Material _mat;
	[SerializeField] private TeleportationPlayer _playerTPScript;

	private Mesh _mesh;
	private float _radianAngle;
	private Vector3 _velocity;
	private Vector3 _curVelocityNorm;

	void Start() {
		_mesh = new Mesh();
		_mesh.MarkDynamic();
		_mesh.name = "Teleportation Arc";
		_mesh.vertices = new Vector3[0];
		_mesh.triangles = new int[0];
	}

	void OnDisable() {
		_tpIndicator.SetActive(false);
		_cantTpIndicator.SetActive(false);
	}

	public void showTPArc(Transform t) {
		this.transform.position = t.position;
		this.transform.rotation = Quaternion.Euler(t.rotation.eulerAngles.x, t.rotation.eulerAngles.y, 180f);
		Vector3 startV = Vector3.forward * _startVelocity;

		_velocity = transform.TransformDirection(startV);
		_curVelocityNorm = _velocity.normalized;
		_angle = clampVelocity();
		makeArcMeshArc();
	}

	private float clampVelocity() {
		//Get forward direction
		Vector3 forwV = _velocity - (Vector3.Project(_velocity, Vector3.up.normalized));

		//find angle between xy and direction
		float angle = Vector3.Angle(forwV, _velocity);
		//Get direction to the right
		Vector3 right = Vector3.Cross(Vector3.up, forwV);

		if (Vector3.Dot(right, Vector3.Cross(forwV, _velocity)) > 0)
			angle *= -1;

		//Clamp angle
		if (angle > 45) {
			_velocity = Vector3.Slerp(forwV, _velocity, 45f / angle);
			_velocity /= _velocity.magnitude;
			_curVelocityNorm = _velocity;
			_velocity *= (Vector3.forward * _startVelocity).magnitude;
			angle = 45f;
		}
		else
			_curVelocityNorm = _velocity.normalized;

		return angle;
	}


	private void makeArcMeshArc() {
		//Creation of vertices and hit information
		bool hitGround = false;
		Vector3 normal;
		List<Vector3> arcVertices = generateArcArray(out normal, out hitGround);

		Vector3 lastPoint = arcVertices[arcVertices.Count - 1];

		hitGround = hitGround && (normal.y > 0.8f) && (normal.x < 0.2f) && (normal.z < 0.2f) ;

		//Displays tp indicator if it's in a valid position
		_tpIndicator.SetActive(hitGround);
		_tpIndicator.transform.position = lastPoint + Vector3.one * 0.05f;
		if (hitGround) {
			_tpIndicator.transform.rotation = Quaternion.LookRotation(normal);
			_tpIndicator.transform.Rotate(90f, 0f, 0f);
		}
		//Displays cant tp indicator if it's in a invalid position
		_cantTpIndicator.SetActive(!hitGround);
		_cantTpIndicator.transform.position = lastPoint + Vector3.one * 0.05f;
		if (!hitGround) {
			_cantTpIndicator.transform.rotation = Quaternion.LookRotation(normal);
			_cantTpIndicator.transform.Rotate(90f, 0f, 0f);
		}

		_playerTPScript.canTeleport(hitGround);


		//Mesh Variables
		Vector3[] vertices = new Vector3[arcVertices.Count * 2];
		Vector2[] uv = new Vector2[arcVertices.Count * 2];
		int[] triangles = new int[6 * (vertices.Length - 2)];

		Vector3 right = Vector3.Cross(_velocity, Vector3.up).normalized;
		float uvOffset = Time.time % 1;

		//Creating mesh
		for (int i = 0; i < arcVertices.Count; i++) {
			vertices[i * 2] = arcVertices[i] - right * _width / 2f;
			vertices[(i * 2) + 1] = arcVertices[i] + right * _width / 2f;

			float uvC = uvOffset;

			if (i == (arcVertices.Count - 1) && i > 1) {
				float lastDist = (arcVertices[i - 2] - arcVertices[i - 1]).magnitude;
				float curDist = (arcVertices[i] - arcVertices[i - 1]).magnitude;
				uvC += 1 - curDist / lastDist;
			}

			uv[i * 2] = new Vector2(0, i - uvC);
			uv[(i * 2) + 1] = new Vector2(1, i - uvC);
		}

		for (int i = 0; i < vertices.Length / 2 - 1; i++) {
			triangles[i * 12]       = triangles[(i * 12) + 8]  = i * 2;
			triangles[(i * 12) + 1] = triangles[(i * 12) + 4]  = (i * 2) + 1;
			triangles[(i * 12) + 7] = triangles[(i * 12) + 10] = (i * 2) + 1;
			triangles[(i * 12) + 2] = triangles[(i * 12) + 3]  = (i * 2) + 2;
			triangles[(i * 12) + 6] = triangles[(i * 12) + 11] = (i * 2) + 2;
			triangles[(i * 12) + 5] = triangles[(i * 12) + 9]  = (i * 2) + 3;
		}


		_mesh.Clear();
		_mesh.vertices = vertices;
		_mesh.triangles = triangles;
		_mesh.uv = uv;
		_mesh.RecalculateBounds();
		_mesh.RecalculateNormals();

		Graphics.DrawMesh(_mesh, Matrix4x4.identity, _mat, gameObject.layer);
	}

	//Creates a List of vector3 (mesh position)
	private List<Vector3> generateArcArray(out Vector3 normal, out bool hitGround) {
		List<Vector3> v = new List<Vector3>();
		Vector3 prev = transform.position;
		v.Add(prev);

		float t = 0f;
		for (int i = 0; i <= _resolution; i++) {
			Vector3 temp = Vector3.zero;
			temp.x = _velocity.x;
			temp.y = _velocity.y + (_gravity * t);
			temp.z = _velocity.z;

			t += 0.5f / temp.magnitude;

			Vector3 next;
			next.x = transform.position.x + (_velocity.x * t);
			next.y = transform.position.y + (_velocity.y * t) + (0.5f * _gravity * t * t);
			next.z = transform.position.z + (_velocity.z * t);

			Ray ray = new Ray(prev, next - prev);
			RaycastHit hitInfo;

			if (Physics.Raycast(ray, out hitInfo, Vector3.Distance(prev, next), _layerMask)) {
				v.Add(hitInfo.point);
				normal = hitInfo.normal;
				hitGround = true;
				_playerTPScript.setTPLocation(hitInfo.point);
				return v;
			}
			else 
				v.Add(next);

			prev = next;
		}

		normal = Vector3.up;
		hitGround = false;
		_playerTPScript.setTPLocation(new Vector3(0, -100, 0));
		return v;
	}

	public void setTPIndicatorRotation(Quaternion rot) {
		_tpIndicator.transform.rotation = rot;
	}

}
