using Luminosity.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubShop : MonoBehaviour
{

    #pragma warning disable 0649
    [Header("Shop Panels")]
    [SerializeField] InformationPanel informationPanel;
    public ShopPanel mainPanel;
    [SerializeField] ShopPanel[] panels;
    #pragma warning restore 0649

    [HideInInspector] public bool isMovingPanels;

    private GameObject playerHud;

    #region weapon info
    [HideInInspector] public static WeaponInformation swordInfo 
        = new WeaponInformation(
            "Sword", 
            "The old faithful of many an adventurer. It slices and dices and not much else. But what if it was on fire?");

    [HideInInspector] public static WeaponInformation bowInfo
        = new WeaponInformation(
            "Bow", 
            "The problem with ranged weapons is it takes time to load. This upgrade keeps your enemies frosty so you can take your time.");

    [HideInInspector] public static WeaponInformation hammerInfo
        = new WeaponInformation(
            "Hammer",
            "Keep it simple taken to a stupid degree. Why use a hammer when a rock on a stick deals more damage?");

    [HideInInspector] public static WeaponInformation spearInfo
        = new WeaponInformation(
            "Spear",
            "If only your spear could stab more than one pot. This upgrade is the closest we could get. Any bystanders to your rampage will be in for a nasty shock.");

    [HideInInspector] public static WeaponInformation crossbowInfo
        = new WeaponInformation(
            "Crossbow",
            "We started from the ground up with this one. We call it a magic missile. Still does the same thing though, just Bigger.");

    [HideInInspector]
    public static WeaponInformation magicInfo
        = new WeaponInformation(
            "Magic",
            "Behold. Magic Magic. It's like Magic but more magical. Also more damage and just all around better.");
    #endregion

    private void Awake()
    {
        informationPanel.Show(mainPanel.weaponName);
        if (playerHud == null) playerHud = FindObjectOfType<PlayerHud>().gameObject;
    }

    #region Navigation
    private void Update()
    {
        if(InputManager.GetButtonDown("Pause Menu"))
        {
            DeactivateHubShop();
        }

        CheckWeaponScroll();

        if (isMovingPanels)
        { HideInformation(); }
        else
        { ShowInformation(); }

        DebugStuff();
    }

    private void ShowInformation()
    {
        if (!informationPanel.gameObject.activeSelf)
        {
            informationPanel.Show(mainPanel.weaponName);
        }
    }

    private void HideInformation()
    {
        if (informationPanel.gameObject.activeSelf)
        {
            informationPanel.Hide();
        }
    }

    private void CheckWeaponScroll()
    {      
        float curHorizontal = InputManager.GetAxisRaw("Horizontal Movement");

        if (curHorizontal != 0 && !isMovingPanels)
        {
            isMovingPanels = true;

            if(curHorizontal > 0)
            //Go Back
            {
                for (int i = 0; i < panels.Length; i++)
                {
                    ShopPanel nextPanel = panels[(i + 1) % panels.Length];
                    nextPanel.StartMovingTo(panels[i].rectTransform.localPosition, panels[i].rectTransform.localScale, panels[i].isMainPanel);
                }
            }
            else
            //Go Forward
            {
                for (int i = 0; i < panels.Length; i++)
                {
                    ShopPanel nextPanel = panels[(i + 1) % panels.Length];
                    panels[i].StartMovingTo(nextPanel.rectTransform.localPosition, nextPanel.rectTransform.localScale, nextPanel.isMainPanel);
                }
            }
        }
    }
    #endregion

    #region Button Calls
    public bool ChargePlayer(int amount)
    {
        Debug.Log($"Charged the Player {amount}.");
        return true;
    }

    public void Upgrade(string weaponName)
    {
        Debug.Log($"Upgraded {weaponName}.");
        GetWeaponInfo(weaponName).isUpgraded = true;
    }

    public void ActivateHubShop()
    {
        playerHud.SetActive(false);

        Player.Instance.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameObject.SetActive(true);
    }

    public void DeactivateHubShop()
    {
        playerHud.SetActive(true);

        Player.Instance.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameObject.SetActive(false);
    }
    #endregion

    #region Utility
    public static WeaponInformation GetWeaponInfo(string weaponName)
    {
        switch(weaponName)
        {
            case "Sword":       return swordInfo;
            case "Bow":         return bowInfo;
            case "Hammer":      return hammerInfo;
            case "Spear":       return spearInfo;
            case "Crossbow":    return crossbowInfo;
            case "Magic":       return magicInfo;
        }

        throw new System.ArgumentException($"No weapon with name {weaponName} exists");
    }

    public void DebugStuff()
    {
        if(InputManager.GetKeyDown(KeyCode.Space))
        {
            GetWeaponInfo(mainPanel.weaponName).isUnlocked = true;
        }
    }
    #endregion
}

public class WeaponInformation
{
    public string name;
    public string description;

    public bool isUnlocked;
    public bool isUpgraded;

    public WeaponInformation(string name, string description)
    {
        this.name = name;
        this.description = description;

        isUnlocked = false;
        isUpgraded = false;
    }
}
