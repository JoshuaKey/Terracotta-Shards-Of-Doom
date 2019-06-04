using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    #pragma warning disable 0649
    [Header("Menus")]
    [SerializeField] GameObject pauseStart;
    [SerializeField] GameObject progress;
    [SerializeField] GameObject options;
    [SerializeField] GameObject video;
    [SerializeField] new GameObject audio;
    [SerializeField] GameObject controls;
	[SerializeField] GameObject gameplay;
    [SerializeField] GameObject quit;
    [Space]
    [Header("Audio")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [Space]
    [Header("Other")]
    [SerializeField] Button continueButton;
    [SerializeField] GameObject playerHud;
    [SerializeField] int currDifficulty = 0;
    public EventSystem eventSystem;
    #pragma warning restore 0649

	public enum Difficulty
	{
		Easy = 15,
		Medium = 11,
		Hard = 6,
		Impossible = 3
	};

    public static PauseMenu Instance;

    // volumes
    [HideInInspector] public float masterVolume = 0f;
    [HideInInspector] public float musicVolume = 0f;
    [HideInInspector] public float sfxVolume = 0f;

    private void Start()
    {
        if (Instance != null) { Destroy(this); return; }
        Instance = this;

        if (eventSystem == null) { eventSystem = FindObjectOfType<EventSystem>(); }

        DeactivatePauseMenu();

        Settings.OnLoad += OnSettingsLoad;
    }

    private void Update() 
    {
        if (InputManager.GetButtonDown("Pause Menu")) 
        {
            PauseMenu.Instance.DeactivatePauseMenu();
        }
    }

    public void OnSettingsLoad(Settings settings) {
        ChangeDifficulty(settings.DifficultyLevel);
        Player.Instance.SkipTutorial = settings.SkipTutorial;
    }

    #region Button Noises
    public void ConfirmNoise()
    {
        AudioManager.Instance.PlaySound("ui_confirm", ESoundChannel.MASTER);
    }

    public void CancelNoise()
    {
        AudioManager.Instance.PlaySound("ui_cancel", ESoundChannel.MASTER);
    }
    #endregion

    #region Navigation
    public void ActivatePauseMenu()
    {
        pauseStart.SetActive(true);
        progress.SetActive(false);
        options.SetActive(false);
        video.SetActive(false);
        audio.SetActive(false);
        controls.SetActive(false);
		gameplay.SetActive(false);
        quit.SetActive(false);

        masterSlider.value = (masterVolume / 100) + 0.8f;
        musicSlider.value = (musicVolume / 100) + 0.8f;
        sfxSlider.value = (sfxVolume / 100) + 0.8f;
        audioMixer.SetFloat("SFXVolume", -80f);

        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (Player.Instance != null)
        {
            //playerCanAttack = Player.Instance.CanAttack;
            //Player.Instance.CanAttack = false;

            //playerCanSwapWeapon = Player.Instance.CanSwapWeapon;
            //Player.Instance.CanSwapWeapon = false;

            Player.Instance.enabled = false;
        }
        playerHud.SetActive(false);

        eventSystem.SetSelectedGameObject(continueButton.gameObject);

        gameObject.SetActive(true);
    }

    public void DeactivatePauseMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        audioMixer.SetFloat("SFXVolume", sfxVolume);

		//print(Player.Instance);
        if (Player.Instance != null)
        {
            //Player.Instance.CanAttack = playerCanAttack;

            //Player.Instance.CanSwapWeapon = playerCanSwapWeapon;
            Player.Instance.enabled = true;
        }

        playerHud.SetActive(true);
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
		gameplay.SetActive(false);
        quit.SetActive(false);

        switch (menuName)
        {
            case "pausestart":
                pauseStart.SetActive(true);
                break;
            case "progress":
                progress.SetActive(true);
                progress.GetComponent<ProgressMenu>().UpdatePercents();
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
			case "gameplay":
				gameplay.SetActive(true);
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
		Game.Instance.SavePlayerStats();
        LevelManager.Instance.LoadScene(sceneName);
    }

    public void Quit()
    {
		//Saving the Player's progress before quitting
		Game.Instance.SavePlayerStats();

        Application.Quit();
    }

    public void QuitToMainMenu() {
        //Saving the Player's progress before quitting
        Game.Instance.SavePlayerStats();

        LevelManager.Instance.LoadScene("MainMenu");
    }

    #endregion

    #region Video
    public void SetResolution(int option)
    {
        switch(option)
        {
            case 0:
                Screen.SetResolution(2560, 1440, Screen.fullScreenMode);
                Debug.Log("Resolution changed to 2560 x 1440.");
                break;
            case 1:
                Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
                Debug.Log("Resolution changed to 1920 x 1080.");
                break;
            case 2:
                Screen.SetResolution(1600, 900, Screen.fullScreenMode);
                Debug.Log("Resolution changed to 1600 x 900.");
                break;
            case 3:
                Screen.SetResolution(1280, 720, Screen.fullScreenMode);
                Debug.Log("Resolution changed to 1280 x 720.");
                break;
            case 4:
                Screen.SetResolution(1024, 576, Screen.fullScreenMode);
                Debug.Log("Resolution changed to 1024 x 576.");
                break;
            case 5:
                Screen.SetResolution(800, 450, Screen.fullScreenMode);
                Debug.Log("Resolution changed to 800 x 450.");
                break;
            case 6:
                Screen.SetResolution(256, 144, Screen.fullScreenMode);
                Debug.Log("Resolution changed to 256 x 144.");
                break;
        }
    }

    public void SetFullScreen(int option)
    {
        switch(option)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Debug.Log("Screen mode set to Exclusive Full Screen.");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                Debug.Log("Screen mode set to Maximized Window.");
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Debug.Log("Screen mode set to Windowed.");
                break;
        }
    }
    #endregion

    #region Volume
    public void ChangeMasterVolume(float volume)
    {
        masterVolume = (volume * 100) - 80;
        audioMixer.SetFloat("MasterVolume", masterVolume);
    }

    public void ChangeMusicVolume(float volume)
    {
        musicVolume = (volume * 100) - 80;
        audioMixer.SetFloat("MusicVolume", musicVolume);
    }

    public void ChangeSFXVolume(float volume)
    {
        sfxVolume = (volume * 100) - 80;
    }
	#endregion

	#region Gameplay

	public void ChangeDifficulty(int index)
	{
        currDifficulty = index;
		switch (index)
		{
			case 0:
                Player.Instance.health.MaxHealth = (float)Difficulty.Easy;
				break;
			case 1:
                Player.Instance.health.MaxHealth = (float)Difficulty.Medium;
				break;
			case 2:
                Player.Instance.health.MaxHealth = (float)Difficulty.Hard;
				break;
			case 3:
                Player.Instance.health.MaxHealth = (float)Difficulty.Impossible;
				break;
		}

		if(Player.Instance.health.CurrentHealth >= Player.Instance.health.MaxHealth)
		{
			Player.Instance.health.Reset();
		}
	}

    public int GetDifficulty() {
        return currDifficulty;
    }

	public void ToggleTutorials(bool value)
	{
		Player.Instance.SkipTutorial = !value;
	}

    public bool GetSkipTutorial() {
        return Player.Instance.SkipTutorial;
    }

    #endregion
}
