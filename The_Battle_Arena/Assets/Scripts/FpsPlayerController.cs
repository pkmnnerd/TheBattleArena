using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

//https://github.com/Brackeys/MultiplayerFPS-Tutorial/blob/master/MultiplayerFPS/Assets/Scripts/PlayerMotor.cs
public class FpsPlayerController : NetworkBehaviour
{
    [SyncVar]
    public int team = 0;


    public Sprite playerRed;
    public Sprite playerBlue;

    public GameObject flashPrefab;
    public GameObject bulletPrefab;
    public GameObject beamPrefab;
    public Transform bulletSpawn;
    public float sensitivity = 1.0f;

    private Vector3 rotation = Vector3.zero;
    public float cameraRotationLimit = 85f;
    private float cameraRotationX = 0f;
    private float currentCameraRotationX = 0f;

    [SyncVar]
    public float speed = 5;

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
                portrait.GetComponent<Image>().sprite = playerRed;
            else
                portrait.GetComponent<Image>().sprite = playerBlue;
            GameObject.Find("CanvasCommander").SetActive(false);
            transform.Find("Healthbar Canvas").gameObject.SetActive(false);
        }
    }

    [Command]
    void CmdSpawnFlash()
    {
        var flash = (GameObject)Instantiate(
           flashPrefab,
           bulletSpawn.position,
           bulletSpawn.rotation);

        NetworkServer.Spawn(flash);
        Destroy(flash, 2f);
    }

    [Command]
    void CmdSpawnLaser(Vector3[] positions)
    {
        var beam = (GameObject)Instantiate(
        beamPrefab,
        bulletSpawn.position,
        bulletSpawn.rotation);

        beam.GetComponent<BeamController>().point1 = positions[0];
        beam.GetComponent<BeamController>().point2 = positions[1];
        NetworkServer.Spawn(beam);
        
        Destroy(beam, 0.1f);
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
            transform.GetComponent<AudioSource>().Play();

            Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            CmdSpawnFlash();

            if (Physics.Raycast(ray, out hit, 1000))
            {
                Vector3[] positions = new Vector3[2];
                positions[0] = transform.Find("GunParent").Find("Bullet Spawn").position;
                positions[1] = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                Debug.Log(positions[0]);
                Debug.Log(positions[1]);

                CmdSpawnLaser(positions);

                float baseDamage = Mathf.Clamp(-Vector3.Distance(transform.position, hit.point) / 6 + (75f / 9f)+15, 0, 15);

                Debug.Log(baseDamage);
                if (hit.collider != null)
                {
                    CmdShoot(hit.collider.transform.gameObject, (int)baseDamage);
                }
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
    void CmdShoot(GameObject hit, int baseDamage)
    {

        Debug.Log(hit);
        if (hit.GetComponent<Health>() != null)
        {
            hit.GetComponent<Health>().TakeDamage(baseDamage);
        }
        if (hit.GetComponent<BaseController>() != null)
        {
            hit.GetComponent<BaseController>().TakeDamage(baseDamage*baseAttackMultipler);
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

            transform.Find("GunParent").localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
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
            GameObject.Find("HealthText").GetComponent<Text>().text = "HP: " + currentHealth + "/" + gameObject.GetComponent<Health>().maxHealth;
        }
    }

}


