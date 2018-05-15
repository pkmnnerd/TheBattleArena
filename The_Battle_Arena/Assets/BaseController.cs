using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class BaseController : NetworkBehaviour
{
    
    public int team;

    public const int maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        
    }

    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            EndGame();
        }
    }

    void OnChangeHealth(int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth / 100, healthBar.sizeDelta.y);

        if (isLocalPlayer && gameObject.GetComponent<FpsPlayerController>() != null)
        {
            gameObject.GetComponent<FpsPlayerController>().UpdateHealth(currentHealth);
        }
    }
    

    public void EndGame()
    {
        GameObject.Find("NetworkLobbyManager").GetComponent<NLMScript>().SendReturnToLobby();
    }

}
