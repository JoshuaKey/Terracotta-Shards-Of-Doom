using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #pragma warning disable 0649
    [Header("Menus")]
    [SerializeField] GameObject titleScreen;
    [SerializeField] GameObject menuStart;
    [SerializeField] GameObject saves;
    [Header("Other")]
    [SerializeField] Button continueButton;
    [SerializeField] EventSystem eventSystem;
    #pragma warning restore 0649

    private void Awake()
    {
        eventSystem.SetSelectedGameObject(continueButton.gameObject);
    }

    public void OpenMainMenu(string menuName)
    {
        titleScreen.SetActive(false);
        menuStart.SetActive(false);
        saves.SetActive(false);

        switch(menuName)
        {
            case "titlescreen":
                titleScreen.SetActive(true);
                break;
            case "menuStart":
                menuStart.SetActive(true);
                break;
            case "saves":
                saves.SetActive(true);
                break;
            default:
                throw new System.Exception("Blame Zac. Or maybe someone else");
                break;
        }
    }

    public void NewGame()
    {
        Debug.Log("New Game");
    }

    public void Continue()
    {
        Debug.Log("Continue");
    }
}
