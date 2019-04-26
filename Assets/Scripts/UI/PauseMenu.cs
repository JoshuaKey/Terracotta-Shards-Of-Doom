using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Space]
    [SerializeField] GameObject PlayerHUD;
    [Space]
    [SerializeField] GameObject PauseStart;
    [SerializeField] GameObject Progress;
    [SerializeField] GameObject Options;

    public static GameObject Instance;

    private void Start()
    {
        if (Instance != null) { Destroy(this); return; }

        Instance = gameObject;

        gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        PauseStart.SetActive(true);
        Progress.SetActive(false);
        Options.SetActive(false);

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Player.Instance.CanAttack = false;
        PlayerHUD.SetActive(false);
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Player.Instance.CanAttack = true;
        PlayerHUD.SetActive(true);
    }

    public void OpenPauseMenu(string menuName)
    {
        menuName = menuName.ToLower();

        PauseStart.SetActive(false);
        Progress.SetActive(false);
        Options.SetActive(false);

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
        }
    }

    public void Continue()
    {
        gameObject.SetActive(false);
    }

    public void Quit()
    {
    }
}
