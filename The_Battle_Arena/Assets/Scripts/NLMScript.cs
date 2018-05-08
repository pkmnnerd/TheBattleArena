using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class NLMScript : NetworkLobbyManager {

    public GameObject lobbyPrefab;
    public GameObject fpsPrefab;
    public GameObject commanderPrefab;

    private Dictionary<NetworkConnection, LobbyPlayer> connections;

    private void Start()
    {
        connections = new Dictionary<NetworkConnection, LobbyPlayer>();
    }

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

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject lobbyPlayer = Instantiate(lobbyPrefab);
        
        connections.Add(conn, lobbyPlayer.GetComponent<LobbyPlayer>());
        return lobbyPlayer;
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("Creating game player");
        GameObject player;
        if (connections[conn].role == 0 || connections[conn].role == 4)
        {
            player = (GameObject)Instantiate(commanderPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        }
        else
        {
            player = (GameObject)Instantiate(fpsPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        }
        //NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        Debug.Log(player);
        return player;
        
    }
    
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        Debug.Log("Setting up Player");
        LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
        if (lPlayer.role == 0 || lPlayer.role == 4)
        {
            CommanderController gPlayer = gamePlayer.GetComponent<CommanderController>();
            if (lPlayer.role < 4)
            {
                gPlayer.team = 0;
                gamePlayer.transform.position = new Vector3(0, 0, -65);
            }
            else
            {
                gPlayer.team = 1;
                gamePlayer.transform.position = new Vector3(0, 0, 65);
            }
        }   
        else
        {
            FpsPlayerController gPlayer = gamePlayer.GetComponent<FpsPlayerController>();
            Debug.Log(gPlayer);
            if (lPlayer.role < 4)
            {
                gPlayer.team = 0;
                gPlayer.GetComponent<MeshRenderer>().material.color = Color.red;
                gPlayer.GetComponent<NavMeshAgent>().Warp(new Vector3((lPlayer.role - 2) * 5, 0, -60));
            }
            else
            {
                gPlayer.team = 1;
                gPlayer.GetComponent<MeshRenderer>().material.color = Color.blue;
                gPlayer.GetComponent<NavMeshAgent>().Warp(new Vector3((lPlayer.role - 6) * 5, 0, 60));
                gPlayer.transform.Rotate(0,180,0);
            }
        }

        

        
        return true;
    }

}
