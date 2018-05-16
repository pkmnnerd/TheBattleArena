using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BeamController : NetworkBehaviour {

    [SyncVar]
    public Vector3 point1;

    [SyncVar]
    public Vector3 point2;


    // Use this for initialization
    void Start () {
        Vector3[] positions = new Vector3[2];
        positions[0] = point1;
        positions[1] = point2;
        GetComponent<LineRenderer>().SetPositions(positions);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
