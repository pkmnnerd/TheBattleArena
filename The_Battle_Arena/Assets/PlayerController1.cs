using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour {

    private Rigidbody rb;
    public float speed;
    public float mouseSen;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float moveMouseHorizontal = Input.GetAxis("Mouse X");

        transform.Translate(new Vector3(moveHorizontal * speed, 0.0f, moveVertical * speed));

        transform.Rotate(new Vector3(0.0f, moveMouseHorizontal * mouseSen, 0.0f));



    }
}
