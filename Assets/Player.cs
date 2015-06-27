using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public Planet planet;

	public float gravForce;
	public float movForce;
	public float jumpForce;

	private Rigidbody2D body;
	private Collider2D collider2d;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		collider2d = GetComponent<Collider2D> ();
	}

	void Update() {
		// Check if touching planet
		Collider2D planetCollider = planet.GetComponent<Collider2D> ();
		if (collider2d.IsTouching (planetCollider)) {
			// Jumping
			if (Input.GetKeyDown (KeyCode.W))
				body.AddForce (transform.TransformDirection (new Vector2 (0.0f, jumpForce)));
		}
	}

	void FixedUpdate () {
		// Gravity
		Vector2 g = planet.Gravity (transform.localPosition);
		body.AddForce (g.normalized * gravForce);

		// Side movement
		Vector2 moveVector = transform.TransformDirection(new Vector2 (Input.GetAxisRaw ("Horizontal") * movForce , 0.0f));
		body.AddForce (moveVector);

		// Rotate towards gravity
		float ang = Mathf.Atan2 (g.y, g.x) * 180 / Mathf.PI + 90;
		body.MoveRotation (ang);

	}
}
