using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NLMScript : NetworkLobbyManager {
    
    public void HostGame()
    {
        //SceneManager.LoadScene("Lobby");
        //singleton.ServerChangeScene("Lobby");

        networkPort = 7777;
        StartHost();
        GameObject.Find("LobbyManager").GetComponent<LobbyScript>().SwitchToNameSelect();

    }

    public void JoinGame()
    {
        //SceneManager.LoadScene("Lobby");
        networkPort = 7777;
        networkAddress = "localhost";
        StartClient();
        GameObject.Find("LobbyManager").GetComponent<LobbyScript>().SwitchToNameSelect();
    }

    public override void OnLobbyServerPlayersReady()
    {
        Debug.Log("Everyone Ready");
        base.OnLobbyServerPlayersReady();
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Creating game player");
        return base.OnLobbyServerCreateGamePlayer(conn, playerControllerId);
    }
    
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("Setting up Player");
        PlayerController gPlayer = gamePlayer.GetComponent<PlayerController>();
        LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
        if (lPlayer.role < 4)
        {
            gPlayer.team = 0;
        }
        else
        {
            gPlayer.team = 1;
        }

        if (lPlayer.role == 0 || lPlayer.role == 4)
        {
            gPlayer.commander = true;
        }
        return true;
    }

}
