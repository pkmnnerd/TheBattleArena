using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.AI;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public int maxHealth = 100;
    public bool destroyOnDeath;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = 100;

    public RectTransform healthBar;

    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                currentHealth = maxHealth;

                // called on the Server, invoked on the Clients
                RpcRespawn();
            }
        }
    }

    void OnChangeHealth(int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth *100 / maxHealth, healthBar.sizeDelta.y);

        if (isLocalPlayer && gameObject.GetComponent<FpsPlayerController>() != null)
        {
            gameObject.GetComponent<FpsPlayerController>().UpdateHealth(currentHealth);
        }
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            if (gameObject.GetComponent<FpsPlayerController>() != null)
            {
                int team = gameObject.GetComponent<FpsPlayerController>().team;

                gameObject.GetComponent<NavMeshAgent>().Warp(new Vector3(0, 0, (team*2-1)*60));
            }
        }
    }

}