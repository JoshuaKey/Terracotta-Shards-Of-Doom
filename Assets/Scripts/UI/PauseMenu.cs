using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #pragma warning disable 0649
    [Space]
    [SerializeField] GameObject PlayerHUD;
    [Space]
    [SerializeField] GameObject PauseStart;
    [SerializeField] GameObject Progress;
    [SerializeField] GameObject Options;
    [SerializeField] GameObject Video;
    [SerializeField] GameObject Audio;
    [SerializeField] GameObject Controls;
    [Space]
    [SerializeField] Button ContinueButton;
    #pragma warning restore 0649

    [Header("Other")]
    public EventSystem eventSystem;

    public static PauseMenu Instance;

    private void Start()
    {
        if (Instance != null) { Destroy(this); return; }

        Instance = this;

        DeactivatePauseMenu();

        if (eventSystem == null) { eventSystem = FindObjectOfType<EventSystem>(); }
    }

    public void ActivatePauseMenu()
    {
        PauseStart.SetActive(true);
        Progress.SetActive(false);
        Options.SetActive(false);

        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Player.Instance != null)
        {
            Player.Instance.CanAttack = false;
            Player.Instance.CanSwapWeapon = false;
        }
        PlayerHUD.SetActive(false);

        eventSystem.SetSelectedGameObject(ContinueButton.gameObject);

        gameObject.SetActive(true);
    }

    public void DeactivatePauseMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Player.Instance != null)
        {
            Player.Instance.CanAttack = true;
            Player.Instance.CanSwapWeapon = true;
        }

        PlayerHUD.SetActive(true);
        eventSystem.SetSelectedGameObject(null);

        gameObject.SetActive(false);
    }

    public void OpenPauseMenu(string menuName)
    {
        menuName = menuName.ToLower();

        PauseStart.SetActive(false);
        Progress.SetActive(false);
        Options.SetActive(false);
        Video.SetActive(false);
        Audio.SetActive(false);
        Controls.SetActive(false);

        Debug.Log($"BUtton pressed. Going to {menuName}.");

        switch(menuName)
        {
            case "pausestart":
                PauseStart.SetActive(true);
                break;
            case "progress":
                Progress.SetActive(true);
                break;
            case "options":
                Options.SetActive(true);
                break;
            case "video":
                Video.SetActive(true);
                break;
            case "audio":
                Audio.SetActive(true);
                break;
            case "controls":
                Controls.SetActive(true);
                break;
        }
    }

    public void Continue()
    {
        DeactivatePauseMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
