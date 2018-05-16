using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeCameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    void FixedUpdate()
    {
        transform.Translate(0.2f, 0, 0);
        transform.LookAt(new Vector3(0, 0, 0));
    }
}
