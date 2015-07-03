using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float gravForce;
	public float movForce;
	public float jumpForce;
	public int doubleJumpMax;

	private Rigidbody2D body;
	private BoxCollider2D boxcollider;
	
	private int doubleJump;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		boxcollider = GetComponent<BoxCollider2D> ();
	}

	void Update() {
		// Check if feet on surface
		if (boxcollider.IsTouchingLayers (1)) {
			doubleJump = doubleJumpMax;

			// Jumping
			if (Input.GetKeyDown (KeyCode.W))
				body.AddForce (transform.TransformDirection (new Vector2 (0.0f, jumpForce)));
		} else {
			// Double jumping
			if (Input.GetKeyDown (KeyCode.W) && doubleJump > 0) {
				body.AddForce (transform.TransformDirection (new Vector2 (0.0f, jumpForce)));
				doubleJump -= 1;
			}
		}
	}

	void FixedUpdate () {
		Planet[] planets = UnityEngine.Object.FindObjectsOfType<Planet>() ;
		Vector2 g = new Vector2 (Mathf.Infinity, Mathf.Infinity);
		foreach (Planet planet in planets) {
			// Gravity
			Vector2 newg = planet.Gravity (transform.localPosition);
			if (newg.magnitude < g.magnitude) g = newg;
		}
		body.AddForce (g.normalized * gravForce);

		// Side movement
		Vector2 moveVector = transform.TransformDirection(new Vector2 (Input.GetAxisRaw ("Horizontal") * movForce , 0.0f));
		body.AddForce (moveVector);

		// Rotate towards gravity
		float ang = Mathf.Atan2 (g.y, g.x) * 180 / Mathf.PI + 90;
		var targetRotation = Quaternion.AngleAxis (ang, new Vector3 (0.0f, 0.0f, 1.0f));
		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 10.0f);
	}
}
