using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.AI;

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

    public GameObject minionPrefab;

    public LayerMask movementMask;
    public LayerMask resourceMask;
    private List<GameObject> selected = new List<GameObject>();

    public int oreCount = 0;

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
            GameObject.Find("Crosshairs").SetActive(false);
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
        cameraHeight = Mathf.Clamp(cameraHeight, minHeight, maxHeight);
        transform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z);

        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            transform.Translate(new Vector3(mouseY, 0, -mouseX));
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
                foreach (GameObject minion in selected)
                {
                    minion.GetComponent<MeshRenderer>().material.color = new Color((1 - team), 0f,team, 1);
                }
                selected.Clear();
            } else
            {
                selectionPanel.transform.localScale = new Vector3(Input.mousePosition.x - selectionStart.x, Input.mousePosition.y - selectionStart.y, 0)/1000;
                selectionPanel.transform.position = (Input.mousePosition + selectionStart) / 2;

            }
        } else
        {
            if (mouseDown == true)
            {

                GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
                

                Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(selectionStart);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, movementMask))
                {
                    Ray ray2 = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit2;

                    if (Physics.Raycast(ray2, out hit2, 100, movementMask))
                    {
                        foreach (GameObject minion in minions)
                        {
                            if (Mathf.Min(hit.point.x, hit2.point.x) < minion.transform.position.x
                                && Mathf.Max(hit.point.x, hit2.point.x) > minion.transform.position.x
                                && Mathf.Min(hit.point.z, hit2.point.z) < minion.transform.position.z
                                && Mathf.Max(hit.point.z, hit2.point.z) > minion.transform.position.z
                                && minion.GetComponent<MinionController>().team == team)
                            {
                                selected.Add(minion);
                                minion.GetComponent<MeshRenderer>().material.color = new Color(0.3f*(1-team),0f,0.3f*team,1);
                            }
                                
                        }
                    }
                }



                    Destroy(selectionPanel);
                mouseDown = false;
            }
        }
        
        if (Input.GetKeyDown("1"))
        {
            CmdSpawnMinion(team);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
            
            foreach (GameObject minion in selected)
            {
                MinionController minionController = minion.GetComponent<MinionController>();

                if (minionController.team == team) {
                    Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100, resourceMask))
                    {
                        Debug.Log("Hit resource");
                        minionController.SetTarget(hit.collider.name, hit.point);
                        minionController.mode = 1;
                        minionController.commander = this;
                        minion.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                    }
                    else if (Physics.Raycast(ray, out hit, 100, movementMask))
                    {
                        Debug.Log(hit.point);
                        minionController.SetTarget("", hit.point);
                        minion.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                        minionController.mode = 1;
                    }
                }
            }
        }

	}



    [Command]
    void CmdSpawnMinion(int mTeam)
    {
        var minion = (GameObject)Instantiate(
            minionPrefab,
            new Vector3(Random.Range(-3f,3f),0,(mTeam*2-1)* 60 + Random.Range(-1f, 1f)),
            Quaternion.identity);
        MinionController minionController = minion.GetComponent<MinionController>();
        minionController.team = mTeam;
        minionController.baseLocation = new Vector3(0, 0, (mTeam * 2 - 1) * 60);
        if (mTeam == 0)
        {
            minion.GetComponent<MeshRenderer>().material.color = Color.red;
        } else
        {
            minion.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        NetworkServer.Spawn(minion);
        minion.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        

    }

    public void incrementOre()
    {
        oreCount++;
        canvas.transform.Find("OreCount").GetComponent<Text>().text = "Ore Count: " + oreCount;
    }
}

