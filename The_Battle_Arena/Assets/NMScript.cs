using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NMScript : NetworkManager {

    private static int playerNumber = 0;

    //https://answers.unity.com/questions/1063433/unet-proper-way-to-set-player-team-onserveraddplay.html
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("hello");
        GameObject player = (GameObject)Instantiate(playerPrefab, new Vector3(0,0,0), new Quaternion(0,0,0,0));
        player.GetComponent<PlayerController>().team = playerNumber % 2;
        if(playerNumber < 2)
        {
            player.GetComponent<PlayerController>().commander = false;
        }
        playerNumber++;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public void HostGame()
    {
        SceneManager.LoadScene("Lobby");
        singleton.networkPort = 7777;
        singleton.StartServer();
    }

    public void JoinGame()
    {
        SceneManager.LoadScene("Lobby");
        singleton.networkPort = 7777;
        singleton.networkAddress = "localhost";
        singleton.StartClient();
    }

}
