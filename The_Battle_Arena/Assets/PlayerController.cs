using UnityEngine;
using UnityEngine.Networking;

//https://github.com/Brackeys/MultiplayerFPS-Tutorial/blob/master/MultiplayerFPS/Assets/Scripts/PlayerMotor.cs
public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public int team = 0;
    [SyncVar]
    public bool commander = false;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    private Vector3 rotation = Vector3.zero;
    public float cameraRotationLimit = 85f;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
       
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }

        if (team == 0)
        {
            CmdColorChange(transform.gameObject, Color.red);
        }
        else
        {
            CmdColorChange(transform.gameObject, Color.blue);
        }

        // https://forum.unity.com/threads/how-do-i-give-each-player-their-own-camera.63708/
        // KEEP CAMERA FIRST CHILD OF PLAYER
        Camera cam = transform.GetChild(0).GetComponent<Camera>();
        cam.enabled = true;
        transform.GetChild(0).GetComponent<AudioListener>().enabled = true;

        if (commander)
        {
            cam.transform.position = new Vector3(0f, 25f, 0f);
            cam.transform.LookAt(new Vector3(0f, 0f, 0f));
        }

        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * 3f;

        //Apply rotation
        rotation = _rotation;

        //Calculate camera rotation as a 3D vector (turning around)
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * 3f;

        //Apply camera rotation
        cameraRotationX = _cameraRotationX;

    }

    // This [Command] code is called on the Client …
    // … but it is run on the Server!
    [Command]
    void CmdFire()
    {
        // Create the Bullet from the Bullet Prefab
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        // Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

        // Spawn the bullet on the Clients
        NetworkServer.Spawn(bullet);

        // Destroy the bullet after 2 seconds
        Destroy(bullet, 2.0f);
    }

    public override void OnStartLocalPlayer()
    {

        //GetComponent<MeshRenderer>().material.color = Color.green;
        //if (team == 0)
        //{

        //    CmdColorChange(transform.gameObject, Color.red);
        //}
        //else
        //{

        //    CmdColorChange(transform.gameObject, Color.blue);
        //}
        //if (team == 0)
        //{

        //    GetComponent<MeshRenderer>().material.color = Color.red;
        //} else
        //{

        //    GetComponent<MeshRenderer>().material.color = Color.blue;
        //}
    }
    //https://answers.unity.com/questions/1063433/unet-proper-way-to-set-player-team-onserveraddplay.html

    [Command]
    void CmdColorChange(GameObject obj, Color toChange)
    {
        RpcColorChange(obj, toChange);
    }

    [ClientRpc]
    void RpcColorChange(GameObject obj, Color toChange)
    {
        obj.GetComponent<MeshRenderer>().material.color = toChange;
    }

    private void FixedUpdate()
    {
        if (!commander) {
            PerformRotation();
        }
    }

    //FIX LATER
    void PerformRotation()
    {
        Camera cam = transform.GetChild(0).GetComponent<Camera>();

        Rigidbody rb = transform.GetComponent<Rigidbody>();

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            // Set our rotation and clamp it
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            //Apply our rotation to the transform of our camera
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}


