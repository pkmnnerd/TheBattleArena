using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonController : MonoBehaviour {

    public int functionNum;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PerformFunction()
    {
        Debug.Log("test");
        CommanderController commander = Camera.main.transform.parent.GetComponent<CommanderController>();
        if (functionNum == 1)
        {
            commander.SpawnMinion();
        }
        else if (functionNum == 2)
        {
            commander.SpawnFighter();
        }
        else if (functionNum == 3)
        {
            commander.UpgradeHealth();
        }
        else if (functionNum == 4)
        {
            commander.UpgradeSpeed();
        }
        else if (functionNum == 5)
        {
            commander.UpgradeDamage();
        }
        else if (functionNum == 6)
        {
            commander.RepairBase();
        }
    }
}
