using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NMScript : NetworkManager {

    private static int playerNumber = 0;

    //https://answers.unity.com/questions/1063433/unet-proper-way-to-set-player-team-onserveraddplay.html
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, new Vector3(0,0,0), new Quaternion(0,0,0,0));
        player.GetComponent<PlayerController>().team = playerNumber % 2;
        if(playerNumber < 2)
        {
            player.GetComponent<PlayerController>().commander = true;
        }
        playerNumber++;
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

}
