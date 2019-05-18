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
    [SerializeField] GameObject pauseStart;
    [SerializeField] GameObject progress;
    [SerializeField] GameObject options;
    [SerializeField] GameObject video;
    [SerializeField] new GameObject audio;
    [SerializeField] GameObject controls;
    [SerializeField] GameObject quit;
    [Space]
    [SerializeField] Button continueButton;
    #pragma warning restore 0649

    [Header("Other")]
    public EventSystem eventSystem;

    public static PauseMenu Instance;

    private bool playerCanAttack = true;
    private bool playerCanSwapWeapon = false;

    private void Start()
    {
        if (Instance != null) { Destroy(this); return; }

        Instance = this;

        DeactivatePauseMenu();

        if (eventSystem == null) { eventSystem = FindObjectOfType<EventSystem>(); }
    }

    public void ActivatePauseMenu()
    {
        pauseStart.SetActive(true);
        progress.SetActive(false);
        options.SetActive(false);
        video.SetActive(false);
        audio.SetActive(false);
        controls.SetActive(false);
        quit.SetActive(false);

        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Player.Instance != null)
        {
            playerCanAttack = Player.Instance.CanAttack;
            Player.Instance.CanAttack = false;

            playerCanSwapWeapon = Player.Instance.CanSwapWeapon;
            Player.Instance.CanSwapWeapon = false;
        }
        PlayerHUD.SetActive(false);

        eventSystem.SetSelectedGameObject(continueButton.gameObject);

        gameObject.SetActive(true);
    }

    public void DeactivatePauseMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (Player.Instance != null)
        {
            Player.Instance.CanAttack = playerCanAttack;

            Player.Instance.CanSwapWeapon = playerCanSwapWeapon;
        }

        PlayerHUD.SetActive(true);
        eventSystem.SetSelectedGameObject(null);

        gameObject.SetActive(false);
    }

    public void OpenPauseMenu(string menuName)
    {
        menuName = menuName.ToLower();

        pauseStart.SetActive(false);
        progress.SetActive(false);
        options.SetActive(false);
        video.SetActive(false);
        audio.SetActive(false);
        controls.SetActive(false);
        quit.SetActive(false);

        Debug.Log($"BUtton pressed. Going to {menuName}.");

        switch(menuName)
        {
            case "pausestart":
                pauseStart.SetActive(true);
                break;
            case "progress":
                progress.SetActive(true);
                break;
            case "options":
                options.SetActive(true);
                break;
            case "video":
                video.SetActive(true);
                break;
            case "audio":
                audio.SetActive(true);
                break;
            case "controls":
                controls.SetActive(true);
                break;
            case "quit":
                quit.SetActive(true);
                break;
        }
    }

    public void Continue()
    {
        DeactivatePauseMenu();
    }

    public void LoadScene(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
