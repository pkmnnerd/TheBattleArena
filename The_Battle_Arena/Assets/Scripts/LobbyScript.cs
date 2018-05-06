using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyScript : MonoBehaviour {


    public GameObject joinMenu;
    public GameObject nameSelect;
    public GameObject roleSelect;
    public InputField nameField;
    
    public Button redCommander;
    public Button redPlayer1;
    public Button redPlayer2;
    public Button redPlayer3;
    
    public Button blueCommander;
    public Button bluePlayer1;
    public Button bluePlayer2;
    public Button bluePlayer3;

    

    // Use this for initialization
    void Start () {
        nameSelect.SetActive(false);
        roleSelect.SetActive(false);
	}
	
	// Update is called once per frame
    
	void Update () {
        for (int i = 0; i < 8; i++)
        {
            Button button = GetButtonByRoleNum(i);
            button.interactable = true;
            button.GetComponentInChildren<Text>().text = "Select";
        }
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        foreach (GameObject playerObject in gameObjects)
        {
            LobbyPlayer player = playerObject.GetComponent<LobbyPlayer>();
            if (player.role >= 0)
            {
                Button button = GetButtonByRoleNum(player.role);
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = player.playerName;
            }
        }

	}
    
    public void UpdateButtons()
    {
        for (int i = 0; i < 8; i++)
        {
            Button button = GetButtonByRoleNum(i);
            button.interactable = true;
            button.GetComponentInChildren<Text>().text = "Select";
        }
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        foreach (GameObject playerObject in gameObjects)
        {
            LobbyPlayer player = playerObject.GetComponent<LobbyPlayer>();
            if (player.role >= 0)
            {
                Button button = GetButtonByRoleNum(player.role);
                button.interactable = false;
                button.GetComponentInChildren<Text>().text = player.playerName;
            }
        }
    }
    
    public void SwitchToNameSelect()
    {
        joinMenu.SetActive(false);
        nameSelect.SetActive(true);
    }

    public void SwitchToRoleSelect()
    {
        nameSelect.SetActive(false);
        roleSelect.SetActive(true);
        UpdateButtons();
    }

    public void SetRole(int roleNum, string name)
    {
        //Button buttonToChange = GetButtonByRoleNum(roleNum);

        //buttonToChange.interactable = false;
        //buttonToChange.GetComponentInChildren<Text>().text = name;

    }

    private Button GetButtonByRoleNum(int roleNum)
    {
        Button buttonToChange;
        switch (roleNum)
        {
            case 0:
                buttonToChange = redCommander;
                break;
            case 1:
                buttonToChange = redPlayer1;
                break;
            case 2:
                buttonToChange = redPlayer2;
                break;
            case 3:
                buttonToChange = redPlayer3;
                break;
            case 4:
                buttonToChange = blueCommander;
                break;
            case 5:
                buttonToChange = bluePlayer1;
                break;
            case 6:
                buttonToChange = bluePlayer2;
                break;
            default:
                buttonToChange = bluePlayer3;
                break;

        }

        return buttonToChange;
    }

    public void SetName()
    {
        string name = nameField.text;
        Debug.Log("Name: " + name);
        if (name.Length > 0 && name.Length <= 32)
        {
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LobbyPlayer");
            foreach (GameObject playerObject in gameObjects)
            {
                LobbyPlayer player = playerObject.GetComponent<LobbyPlayer>();
                if (player.isLocalPlayer)
                {
                    player.SetName(name);
                }
            }
            SwitchToRoleSelect();
        }
    }

    public void SetRole(int roleNum)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        foreach (GameObject playerObject in gameObjects)
        {
            LobbyPlayer player = playerObject.GetComponent<LobbyPlayer>();
                
            if (player.isLocalPlayer)
            {
                player.SetRole(roleNum);
            }
        }
    }

}
