using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class MinionController : NetworkBehaviour {

    [SyncVar]
    public int team;
    public string target;
    public Vector3 dest;
    //0 idle, 1 move, 2 action, 3 move to base
    public int mode = 0;
    private float time = 0;
    [SyncVar]
    public Vector3 baseLocation;
    //0 nothing, 1 rock
    public int resourceType = 0;
    public CommanderController commander;

	// Use this for initialization
	void Start () {
        if (team == 0)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.blue;

        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!hasAuthority)
        {
            return;
        }
        if (mode == 0)
        {

        }else if (mode == 1)
        {
            if (Vector3.Distance(transform.position, dest) < 3)
            {
                GetComponent<NavMeshAgent>().ResetPath();
                if (target.Equals(""))
                {
                    mode = 0;
                    dest = Vector3.zero;

                }
                else
                {
                    mode = 2;
                    time = 0;
                }
            }
        }else if (mode == 2)
        {
            if (time < 3)
            {
                time += Time.deltaTime;
            }
            else
            {
                if (target.Equals("Rock"))
                {
                    resourceType = 1;
                }
                dest = baseLocation;
                transform.GetComponent<NavMeshAgent>().SetDestination(baseLocation);
                mode = 3;
            }
        } else if (mode == 3)
        {
            if (Vector3.Distance(transform.position, dest) < 3)
            {
                if (target.Equals("Rock"))
                {
                    commander.incrementOre();
                }
                resourceType = 0;
                mode = 1;
                time = 0;
                dest = GameObject.Find(target).transform.position;
                GetComponent<NavMeshAgent>().SetDestination(dest);
            }
        }
    }

    public void SetTarget(string target, Vector3 dest)
    {
        this.target = target;
        this.dest = dest;
    }


    [Command]
    void CmdUpdateTarget()
    {
    }
    
}
