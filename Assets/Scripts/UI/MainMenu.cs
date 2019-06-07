using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #pragma warning disable 0649
    [Header("Menus")]
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject menuStart;
    [SerializeField] GameObject credits;
    [Header("Other")]
    [SerializeField] Button continueButton;
    [SerializeField] EventSystem eventSystem;
    #pragma warning restore 0649

    private void Start()
    {
		//eventSystem = FindObjectOfType<EventSystem>();
        //eventSystem.SetSelectedGameObject(continueButton.gameObject);
		//Player.Instance.enabled = false;
		//Player.Instance.HideWeapon();
		//PlayerHud.Instance.DisablePlayerHud();
		//PauseMenu.Instance.DeactivatePauseMenu();
		//Cursor.lockState = CursorLockMode.None;
		//Cursor.visible = true;
    }

    public void OpenMainMenu(string menuName)
    {
        titleScreen.SetActive(false);
        menuStart.SetActive(false);
        credits.SetActive(false);

        switch(menuName)
        {
            case "titlescreen":
                titleScreen.SetActive(true);
                break;
            case "menuStart":
                menuStart.SetActive(true);
                continueButton.interactable = File.Exists(Application.dataPath + "/save.json");
                break;
            case "credits":
                credits.SetActive(true);
                break;
            default:
                throw new System.Exception("Blame Zac. Or maybe someone else");
        }
    }

    public void NewGame()
    {
        Debug.Log("New Game");

		DisableMainMenu();

        PlayerPrefs.SetInt("Continue", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Persistent", LoadSceneMode.Single);
    }

    public void Continue()
    {
        Debug.Log("Continue");

		DisableMainMenu();

        PlayerPrefs.SetInt("Continue", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Persistent", LoadSceneMode.Single);

        //Loading the Save file of the player
        //Game.Instance.LoadPlayerStats();
    }

	public void QuitApplication()
	{
		//Saving the Player's Progress
		//Game.Instance.SavePlayerStats();

		Application.Quit();
	}

	private void DisableMainMenu()
	{
		//Disabling the Main Menu
		titleScreen.SetActive(false);
		menuStart.SetActive(false);
		credits.SetActive(false);
	}
}
