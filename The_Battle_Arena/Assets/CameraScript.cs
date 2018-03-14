using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float mouseSenY;
    public Camera cam;
    //private float angle;

	// Use this for initialization
	void Start () {
//        angle = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void FixedUpdate()
    {
        float moveMouseVertical = Input.GetAxis("Mouse Y");
        //angle += moveMouseVertical * mouseSenY;
        //angle = Mathf.Clamp(angle, -90, 90);
        //cam.transform.rotation = Quaternion.Euler (angle, cam.transform.rotation.y, 0);

        float rotation = -moveMouseVertical * mouseSenY;
        if (cam.transform.localEulerAngles.x < 180 && cam.transform.localEulerAngles.x + rotation > 90)
        {
            rotation = 90 - cam.transform.localEulerAngles.x;
        }
        if (cam.transform.localEulerAngles.x > 180 && cam.transform.localEulerAngles.x + rotation < 270)
        {
           rotation = 270 - cam.transform.localEulerAngles.x;
        }
        transform.Rotate(new Vector3(rotation, 0.0f, 0.0f));
    }
}
