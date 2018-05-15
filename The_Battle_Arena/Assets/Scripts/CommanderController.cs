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
    public GameObject fighterPrefab;

    public LayerMask movementMask;
    public LayerMask resourceMask;
    public LayerMask attackMask;
    private List<GameObject> selected = new List<GameObject>();

    public int oreCount;
    public int goldCount;

    public int healthUpgradeCount = 0;
    public int speedUpgradeCount = 0;
    public int damageUpgradeCount = 0;


    // Use this for initialization
    void Start() {
        if (isLocalPlayer)
        {
            transform.Find("Main Camera").gameObject.SetActive(true);
            Cursor.visible = true;
            GameObject portrait = GameObject.Find("CommanderPortrait");
            portrait.GetComponent<RectTransform>().localScale = new Vector3(Screen.height * 0.2f / 100, Screen.height * 0.2f / 100, 1);
            portrait.GetComponent<RectTransform>().position = new Vector3(Screen.height * 0.2f / 2, Screen.height * 0.2f / 2, 0);
            if (team == 0)
                portrait.GetComponent<Image>().color = Color.red;
            else
                portrait.GetComponent<Image>().color = Color.blue;
            canvas = GameObject.Find("CanvasCommander").GetComponent<Canvas>();
            GameObject.Find("CanvasFPS").SetActive(false);
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
                                && ((minion.GetComponent<MinionController>() != null && minion.GetComponent<MinionController>().team == team) || (minion.GetComponent<FighterController>() != null && minion.GetComponent<FighterController>().team == team)))
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
        
        
        if (Input.GetMouseButtonDown(1))
        {
            GameObject[] minions = GameObject.FindGameObjectsWithTag("Minion");
            
            foreach (GameObject minion in selected)
            {
                if (minion.GetComponent<MinionController>() != null) {
                    MinionController minionController = minion.GetComponent<MinionController>();

                    if (minionController.team == team)
                    {
                        Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 100, resourceMask))
                        {
                            Debug.Log("Hit resource");
                            minionController.SetTarget(hit.collider.name, hit.point, hit.collider.transform.gameObject);
                            minionController.mode = 1;
                            minionController.commander = this;
                            minion.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                        }
                        else if (Physics.Raycast(ray, out hit, 100, movementMask))
                        {
                            Debug.Log(hit.point);
                            minionController.SetTarget("", hit.point, null);
                            minion.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                            minionController.mode = 1;
                        }
                    }
                }

                if (minion.GetComponent<FighterController>() != null)
                {
                    FighterController fighterController = minion.GetComponent<FighterController>();

                    if (fighterController.team == team)
                    {
                        Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit, 100, attackMask))
                        {
                            Debug.Log("Hit attackable");
                            fighterController.SetTarget(hit.collider.name, hit.point, hit.collider.transform.gameObject);
                            fighterController.mode = 2;
                            fighterController.commander = this;
                            // change this / may not be needed
                            minion.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                        }
                        else if (Physics.Raycast(ray, out hit, 100, movementMask))
                        {
                            Debug.Log(hit.point);
                            fighterController.SetTarget("", hit.point, null);
                            minion.GetComponent<NavMeshAgent>().SetDestination(hit.point);
                            fighterController.mode = 1;
                        }
                    }
                }

            }
        }

        if (Input.GetKeyDown("1") && oreCount >= 5)
        {
            oreCount = oreCount - 5;
            canvas.transform.Find("OreCount").GetComponent<Text>().text = "Ore Count: " + oreCount;
            CmdSpawnMinion(team);
        }


        if (Input.GetKeyDown("2") && oreCount >= 5)
        {
            oreCount = oreCount - 5;
            canvas.transform.Find("OreCount").GetComponent<Text>().text = "Ore Count: " + oreCount;
            CmdSpawnFighter(team);
        }

        // player health
        if (Input.GetKeyDown("3") && healthUpgradeCount <= 2)
        {
            if (oreCount >= 100)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject player in players)
                {
                    if (player.GetComponent<FpsPlayerController>().team == team)
                    {
                        CmdUpgradeHealth(player, 100);
                    }
                }
                oreCount -= 100;
                healthUpgradeCount++; 
            }
        }

        // speed
        if (Input.GetKeyDown("4") && speedUpgradeCount <= 1)
        {
            if (oreCount >= 100)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject player in players)
                {
                    if (player.GetComponent<FpsPlayerController>().team == team)
                    {
                        CmdUpgradeSpeed(player, 5);
                    }
                }
                oreCount -= 100;
                speedUpgradeCount++;
            }
        }

        // damage against base
        if (Input.GetKeyDown("5") && damageUpgradeCount <= 2)
        {
            if (oreCount >= 100)
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject player in players)
                {
                    if (player.GetComponent<FpsPlayerController>().team == team)
                    {
                        CmdUpgradeBaseMultiplier(player, 3);
                    }
                }
                oreCount -= 100;
                damageUpgradeCount++;
            }
        }

        // heal base
        if (Input.GetKeyDown("6"))
        {
            if (oreCount >= 10)
            {
                GameObject[] bases = GameObject.FindGameObjectsWithTag("Base");

                foreach (GameObject baseObj in bases)
                {
                    if (baseObj.GetComponent<BaseController>().team == team)
                    {
                        CmdHealBase(baseObj, 300);
                    }
                }
                oreCount -= 10;
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

    [Command]
    void CmdSpawnFighter(int mTeam)
    {
        var fighter = (GameObject)Instantiate(
            fighterPrefab,
            new Vector3(Random.Range(-3f, 3f), 0, (mTeam * 2 - 1) * 60 + Random.Range(-1f, 1f)),
            Quaternion.identity);
        FighterController fighterController = fighter.GetComponent<FighterController>();
        fighterController.team = mTeam;
        fighterController.baseLocation = new Vector3(0, 0, (mTeam * 2 - 1) * 60);
        if (mTeam == 0)
        {
            fighterController.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            fighterController.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        NetworkServer.Spawn(fighter);
        fighter.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);


    }


    [Command]
    void CmdUpgradeHealth(GameObject player, int upgradeAmount)
    {
        player.GetComponent<Health>().maxHealth += upgradeAmount;
        player.GetComponent<Health>().currentHealth += upgradeAmount;
    }

    [Command]
    void CmdUpgradeSpeed(GameObject player, int upgradeAmount)
    {
        player.GetComponent<FpsPlayerController>().speed += upgradeAmount;
    }

    [Command]
    void CmdUpgradeBaseMultiplier(GameObject player, int upgradeAmount)
    {
        player.GetComponent<FpsPlayerController>().baseAttackMultipler += upgradeAmount;
    }

    [Command]
    void CmdHealBase(GameObject baseObj, int healAmount)
    {
        baseObj.GetComponent<BaseController>().currentHealth = Mathf.Clamp(baseObj.GetComponent<BaseController>().currentHealth + healAmount, 0, 10000);
    }

    public void incrementOre()
    {
        oreCount++;
        canvas.transform.Find("OreCount").GetComponent<Text>().text = "Ore Count: " + oreCount;
    }

    public void incrementGold()
    {
        goldCount++;
        canvas.transform.Find("GoldCount").GetComponent<Text>().text = "Gold Count: " + goldCount;
    }
}

