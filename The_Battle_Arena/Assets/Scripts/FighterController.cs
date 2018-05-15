using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AI;

public class FighterController : NetworkBehaviour
{

    [SyncVar]
    public int team;
    [SyncVar]
    public string target;
    [SyncVar]
    public GameObject targetObj;
    [SyncVar]
    public Vector3 dest;
    //0 idle, 1 move, 2 attack target
    public int mode = 0;
    private float time = 1;

    [SyncVar]
    public Vector3 baseLocation;

    public CommanderController commander;

    // Use this for initialization
    void Start()
    {
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
    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        if (mode == 0)
        {

        }
        else if (mode == 1)
        {
            if (Vector3.Distance(transform.position, dest) < 3)
            {
                GetComponent<NavMeshAgent>().ResetPath();
                if (target.Equals(""))
                {
                    mode = 0;
                    dest = Vector3.zero;

                }
            }
        } else if (mode == 2)
        {
            if (targetObj != null)
            {
                time += Time.deltaTime;
                if (Vector3.Distance(targetObj.transform.position, transform.position) < 1)
                {
                    transform.GetComponent<NavMeshAgent>().ResetPath();
                    if (time >= 1)
                    {
                        CmdDealDamage();
                        time = 0;
                    }
                } else
                {
                    transform.GetComponent<NavMeshAgent>().SetDestination(targetObj.transform.position);
                }
            } else
            {
                mode = 0;
                time = 1;
                transform.GetComponent<NavMeshAgent>().ResetPath();
            }
        }
    }

    public void SetTarget(string target, Vector3 dest, GameObject targetObj)
    {
        CmdUpdateTarget(target, dest, targetObj);
    }


    [Command]
    void CmdUpdateTarget(string target, Vector3 dest, GameObject targetObj)
    {
        this.target = target;
        this.dest = dest;
        this.targetObj = targetObj;
    }

    
    [Command]
    void CmdDealDamage()
    {
        if(targetObj.GetComponent<Health>() != null)
        {
            targetObj.GetComponent<Health>().TakeDamage(5);
        }
        if (targetObj.GetComponent<BaseController>() != null)
        {
            targetObj.GetComponent<BaseController>().TakeDamage(5);
        }
    }
}
