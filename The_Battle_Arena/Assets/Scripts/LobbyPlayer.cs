using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer {

    [SyncVar]
    public string playerName;

    [SyncVar]
    public int team;

    private InputField nameField;

    [SyncVar(hook = "OnChangeRole")]
    public int role = -1;


    private LobbyScript lobbyScript;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Start()
    {
        role = -1;
    }

    [Command]
    public void CmdSetName(string inputName)
    {
        SetName(inputName);
    }

    public void SetName(string inputName)
    {
        if (isServer)
        {
            playerName = inputName;
        }
        else
        {
            CmdSetName(inputName);
        }
    }

    [Command]
    public void CmdSetRole(int roleNum)
    {
        SetRole(roleNum);
    }

    public void SetRole(int roleNum)
    {
        if (isServer)
        {
            role = roleNum;
        } else
        {
            CmdSetRole(roleNum);
        }
        if (isLocalPlayer)
        {
            SendReadyToBeginMessage();
        }
    }
    

    public void OnChangeRole(int newRole)
    {
        role = newRole;
        GameObject.Find("LobbyManager").GetComponent<LobbyScript>().UpdateButtons();
    }
}
