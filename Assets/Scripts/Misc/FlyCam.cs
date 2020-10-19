using UnityEngine;
using System.Collections;

public class FlyCam : MonoBehaviour {

	public float cameraSensitivity = 90;
	public float normalMoveSpeed = 10;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;

	private float rotationX = 0.0f;
	private float rotationY = 0.0f;

	void Start() {
		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update() {
		if (Cursor.lockState == CursorLockMode.Locked) {
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp(rotationY, -90, 90);

			transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
		}

		float moveSpeed = normalMoveSpeed;

		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
			moveSpeed *= fastMoveFactor;
		}
		else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
			moveSpeed *= slowMoveFactor;
		}

		Vector3 moveVector = Vector3.zero;
		if (Input.GetKey(KeyCode.Q))
			moveVector -= transform.right;
		if (Input.GetKey(KeyCode.D))
			moveVector += transform.right;
		if (Input.GetKey(KeyCode.Z))
			moveVector += transform.forward;
		if (Input.GetKey(KeyCode.S))
			moveVector -= transform.forward;
		if (Input.GetKey(KeyCode.A))
			moveVector += transform.up;
		if (Input.GetKey(KeyCode.E))
			moveVector -= transform.up;


		transform.position += moveVector * moveSpeed * Time.deltaTime;
		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Cursor.lockState == CursorLockMode.None) {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
            }
			else {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
            }
		}
	}
}