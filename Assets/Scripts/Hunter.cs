using UnityEngine;
using System.Collections;

public class Hunter : MonoBehaviour {

	public Transform target;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Rotate the camera every frame so it keeps looking at the target
		transform.LookAt(target);

		// alternatively point the object at the world origin
		// transform.LookAt(Vector3.zero)

	
	}
}
