using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//https://github.com/Brackeys/MultiplayerFPS-Tutorial/blob/master/MultiplayerFPS/Assets/Scripts/PlayerMotor.cs
public class FpsPlayerController : NetworkBehaviour
{
    [SyncVar]
    public int team = 0;

    public GameObject flashPrefab;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float sensitivity = 1.0f;

    private Vector3 rotation = Vector3.zero;
    public float cameraRotationLimit = 85f;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    [SyncVar]
    public int speed = 5;

    [SyncVar]
    public int baseAttackMultipler = 1;

    void Start()
    {
        if (isLocalPlayer)
        {
            transform.Find("Main Camera").gameObject.SetActive(true);
            Cursor.visible = false;
            GameObject portrait = GameObject.Find("Portrait");
            portrait.GetComponent<RectTransform>().localScale = new Vector3(Screen.height * 0.2f / 100, Screen.height * 0.2f / 100, 1);
            portrait.GetComponent<RectTransform>().position = new Vector3(Screen.height * 0.2f / 2, Screen.height * 0.2f / 2, 0);
            if (team == 0)
                portrait.GetComponent<Image>().color = Color.red;
            else
                portrait.GetComponent<Image>().color = Color.blue;
            GameObject.Find("CanvasCommander").SetActive(false);
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (team == 0)
        {
            CmdColorChange(transform.gameObject, Color.red);
        }
        else
        {
            CmdColorChange(transform.gameObject, Color.blue);
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        
        transform.Translate(x, 0, z);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    CmdFire();
        //}

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            var flash = (GameObject)Instantiate(
            flashPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);
            
            NetworkServer.Spawn(flash);

            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.collider.transform.gameObject);
                CmdShoot(hit.collider.transform.gameObject);
            }
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

    [Command]
    void CmdShoot(GameObject hit)
    {
        Debug.Log(hit);
        if (hit.GetComponent<Health>() != null)
        {
            hit.GetComponent<Health>().TakeDamage(10);
        }
        if (hit.GetComponent<BaseController>() != null)
        {
            hit.GetComponent<BaseController>().TakeDamage(10*baseAttackMultipler);
        }
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

    public void UpdateHealth(int currentHealth)
    {
        if (isLocalPlayer)
        {
            GameObject.Find("HealthText").GetComponent<Text>().text = "Health: " + currentHealth + "/" + gameObject.GetComponent<Health>().maxHealth;
        }
    }

}


