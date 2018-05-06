using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour {

    public GameObject mainMenuObject;
    public GameObject joinMenuObject;
    
	// Use this for initialization
	void Start () {

        NetworkManager nm = NetworkManager.singleton;

        joinMenuObject.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayGame()
    {
        SceneManager.LoadScene("Lobby");
        //joinMenuObject.SetActive(true);
        //mainMenuObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void Back()
    {
        joinMenuObject.SetActive(false);
        mainMenuObject.SetActive(true);
    }
}
