using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResourceScript : NetworkBehaviour {

    public int resourceType;

    [SyncVar]
    int resourceCount;

	// Use this for initialization
	void Start () {
        resourceCount = 5;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DecrementResource()
    {
        resourceCount--;
        Debug.Log(resourceCount);
        if (resourceCount == 0)
        {
            CmdResetResource();
        }
    }

    [Command]
    private void CmdResetResource()
    {
        resourceCount = 5;
        transform.position = new Vector3(Random.Range(-45f,45f),-1,Random.Range(-20f,20f));
    }

}
