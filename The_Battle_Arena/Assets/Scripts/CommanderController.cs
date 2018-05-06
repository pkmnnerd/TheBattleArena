using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CommanderController : NetworkBehaviour {

    [SyncVar]
    public int team = 0;

    public GameObject selectionPrefab;
    public Canvas canvas;
    public float minHeight;
    public float maxHeight;
    public float zoomSpeed;


    bool mouseDown = false;
    GameObject selectionPanel;
    Vector3 selectionStart;
    private float cameraHeight = 20;

    // Use this for initialization
    void Start() {
        if (isLocalPlayer)
        {
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
            Cursor.visible = true;
            GameObject portrait = GameObject.Find("Portrait");
            if (team == 0)
                portrait.GetComponent<Image>().color = Color.red;
            else
                portrait.GetComponent<Image>().color = Color.blue;
            canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
        {
            return;
        }

        float deltaZoom = Input.GetAxis("Mouse ScrollWheel");
        cameraHeight -= deltaZoom * zoomSpeed;
        Mathf.Clamp(cameraHeight, minHeight, maxHeight);
        transform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z);

        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            transform.Translate(new Vector3(-mouseY, 0, mouseX));
        }
        if (Input.GetMouseButton(0))
        {
            if (mouseDown == false)
            {
                selectionPanel = Instantiate(selectionPrefab, Input.mousePosition, Quaternion.identity, canvas.transform);
                selectionPanel.transform.localScale = new Vector3(0, 0, 1);
                selectionPanel.transform.position = Input.mousePosition;
                selectionStart = Input.mousePosition;
                mouseDown = true;
            } else
            {
                selectionPanel.transform.localScale = new Vector3(Input.mousePosition.x - selectionStart.x, Input.mousePosition.y - selectionStart.y, 0)/1000;
                selectionPanel.transform.position = (Input.mousePosition + selectionStart) / 2;

            }
        } else
        {
            if (mouseDown == true)
            {
                Destroy(selectionPanel);
                mouseDown = false;
            }
        }
        
	}
    

}
