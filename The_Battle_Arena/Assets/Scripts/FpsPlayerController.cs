using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//https://github.com/Brackeys/MultiplayerFPS-Tutorial/blob/master/MultiplayerFPS/Assets/Scripts/PlayerMotor.cs
public class FpsPlayerController : NetworkBehaviour
{
    [SyncVar]
    public int team = 0;

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float sensitivity = 1.0f;

    private Vector3 rotation = Vector3.zero;
    public float cameraRotationLimit = 85f;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    void Start()
    {
        if (isLocalPlayer)
        {
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
            Cursor.visible = false;
            GameObject portrait = GameObject.Find("Portrait");
            if (team == 0)
                portrait.GetComponent<Image>().color = Color.red;
            else
                portrait.GetComponent<Image>().color = Color.blue;
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
       
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 5.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;
        
        transform.Translate(x, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
        

        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * sensitivity;

        //Apply rotation
        rotation = _rotation;

        //Calculate camera rotation as a 3D vector (turning around)
        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * sensitivity;

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


    private void FixedUpdate()
    {
        PerformRotation();
    }

    //FIX LATER
    void PerformRotation()
    {
        Camera cam = transform.GetComponentInChildren<Camera>();

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


