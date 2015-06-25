using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class Planet : MonoBehaviour {

	private struct PlanetSide
	{
		public Vector2 point;
		public Vector2 normal;
		public Vector2 ray;

		public PlanetSide(Vector2 pt, Vector2 n, Vector2 r) {
			point = pt;
			normal = n;
			ray = r;
		}
	}

	private PlanetSide[] sides;
	
	private PolygonCollider2D polygon;

	// Use this for initialization
	void Start () {
		polygon = GetComponent<PolygonCollider2D> ();
		Vector2[] points = polygon.GetPath (0);

		// Get sides
		sides = InitializeSides (points);

		// Set up game object with mesh;
		gameObject.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
		Triangulator tr = new Triangulator(points);
		filter.mesh = tr.GenerateMesh ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach(PlanetSide side in sides) {
			// Draw normals
			Vector2 pt = transform.TransformPoint (side.point);
			Vector2 n = transform.TransformDirection (side.normal);

			Debug.DrawLine (pt, pt + n, Color.red);
		}

		// Draw gravity
		Vector2 mouse = Input.mousePosition;
		mouse = Camera.main.ScreenToWorldPoint (mouse);

		Vector2 g = Gravity (mouse);
		Debug.DrawLine (mouse, mouse + g, Color.green);
	}

	void OnDrawGizmos () {
		polygon = GetComponent<PolygonCollider2D> ();

		Vector2[] points = polygon.GetPath (0);
		Vector2 prevPoint = transform.TransformPoint(points[points.Length-1]);
		Vector2 nextPoint;

		foreach(Vector2 point in points) {
			nextPoint = transform.TransformPoint (point);

			Gizmos.DrawLine (prevPoint, nextPoint);
			prevPoint = nextPoint;
		}
	}

	// PLANET SIDES
	private PlanetSide[] InitializeSides(Vector2[] points) {
		PlanetSide[] sd = new PlanetSide[points.Length];

		Vector2 pt, pt2;

		for (int i = 0; i < points.Length; i++) {
			pt = points[i];
			pt2 = points[(i + 1) % points.Length];

			Vector2 r = pt2 - pt;
			Vector2 n = Vector3.Cross (new Vector3(0.0f, 0.0f, 1.0f), r.normalized);

			sd[i] = new PlanetSide(pt, n, r);
		}

		return sd;
	}

	public Vector2 Gravity(Vector2 point) {
		Vector2 g = new Vector2(0.0f, 0.0f);
		point = transform.InverseTransformPoint (point);

		float minDist = float.MaxValue;
		foreach (PlanetSide side in sides) {
			// Displacement from point
			Vector2 disp = point - side.point;
			if (disp.magnitude < minDist) {
				g = disp * -1; // flip it to point towards planet
				minDist = disp.magnitude;
			}

			// Distance from side
			float x = Vector2.Dot (disp, side.ray.normalized);
			float y = Vector2.Dot (disp, side.normal);
			if (x >= 0.0f && x <= side.ray.magnitude && y >= 0.0f && y < minDist) {
				g = side.normal * -y; // flip to point towards planet and scale by distance
				minDist = y;
			}
		}

		g = transform.TransformDirection (g);
		return g;
	}
}
