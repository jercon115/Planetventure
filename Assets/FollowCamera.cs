using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

	public GameObject target;

	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate((target.transform.localPosition - transform.localPosition ) * 0.1f);
		transform.Translate(new Vector3(0.0f,0.0f, -10 - transform.localPosition.z));
	}
}
