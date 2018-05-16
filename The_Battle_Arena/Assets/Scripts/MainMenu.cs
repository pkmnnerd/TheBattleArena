using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour {

    public GameObject mainMenuObject;
    public GameObject joinMenuObject;
    public GameObject howToPlayMenuObject;
    public GameObject howToPlay2MenuObject;
    public GameObject howToPlay3MenuObject;

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

    public void HowToPlay()
    {
        mainMenuObject.SetActive(false);
        howToPlayMenuObject.SetActive(true);
    }

    public void HowToPlayBack()
    {
        howToPlayMenuObject.SetActive(false);
        mainMenuObject.SetActive(true);
    }

    public void HowToPlay2Back()
    {
        howToPlay2MenuObject.SetActive(false);
        mainMenuObject.SetActive(true);
    }
    public void HowToPlay3Back()
    {
        howToPlay3MenuObject.SetActive(false);
        mainMenuObject.SetActive(true);
    }

    public void Next1()
    {
        howToPlayMenuObject.SetActive(false);
        howToPlay2MenuObject.SetActive(true);
    }

    public void Prev1()
    {
        howToPlay2MenuObject.SetActive(false);
        howToPlayMenuObject.SetActive(true);
    }
    public void Next2()
    {
        howToPlay2MenuObject.SetActive(false);
        howToPlay3MenuObject.SetActive(true);
    }

    public void Prev2()
    {
        howToPlay3MenuObject.SetActive(false);
        howToPlay2MenuObject.SetActive(true);
    }
}
