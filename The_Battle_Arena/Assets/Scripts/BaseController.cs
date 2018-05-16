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
        
        StartCoroutine(Wait());
        
    }

    [ClientRpc]
    public void RpcDisplayWinText()
    {
        if (team == 0)
        {
            GameObject.Find("WinText").GetComponent<Text>().text = "Blue Wins";
        }
        else
        {
            GameObject.Find("WinText").GetComponent<Text>().text = "Red Wins";
        }
    }

    IEnumerator Wait()
    {
        RpcDisplayWinText();
        yield return new WaitForSeconds(3f);
        Application.Quit();
    }

}
