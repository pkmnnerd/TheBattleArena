using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Timer : NetworkBehaviour {

    [SyncVar]
    public float gameTime;

	// Use this for initialization
	void Start () {
        gameTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
        gameTime += Time.deltaTime;

        string time = (int)(gameTime / 60) + ":" + ((int)(gameTime) % 60).ToString("D2");

        GetComponentInChildren<Text>().text = time;
        
	}
}
