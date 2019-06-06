using Luminosity.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HubShop : MonoBehaviour
{

    #pragma warning disable 0649
    [Header("Shop Panels")]
    [SerializeField] InformationPanel informationPanel;
    public ShopPanel mainPanel;
    [SerializeField] ShopPanel[] shopPanels;
    #pragma warning restore 0649

    [HideInInspector] public bool isMovingPanels;
    [HideInInspector] public UnityAction onSuccessfulSale;
    [HideInInspector] public UnityAction onFailedSale;

    //private GameObject playerHud;

    #region weapon info
    [HideInInspector] public static WeaponInformation swordInfo 
        = new WeaponInformation(
            "Sword", 
            "2,000 Coins\nThe old faithful of many an adventurer. It slices and dices and not much else. But what if it was on fire?", true);

    [HideInInspector] public static WeaponInformation bowInfo
        = new WeaponInformation(
            "Bow", 
            "4,000 Coins\nThe problem with ranged weapons is it takes time to load. This upgrade keeps your enemies frosty so you can take your time.");

    [HideInInspector] public static WeaponInformation hammerInfo
        = new WeaponInformation(
            "Hammer",
            "6,000 Coins\nKeep it simple taken to a stupid degree. Why use a hammer when a rock on a stick deals more damage?");

    [HideInInspector] public static WeaponInformation spearInfo
        = new WeaponInformation(
            "Spear",
            "8,000 Coins\nWish you could stab more than one pot? This upgrade is the closest we could get. Any bystander pots will be in for a nasty shock.");

    [HideInInspector] public static WeaponInformation crossbowInfo
        = new WeaponInformation(
            "Crossbow",
            "12,500 Coins\nWe started from the ground up with this one. We call it a magic missile. Still does the same thing though, just Bigger.");

    [HideInInspector]
    public static WeaponInformation magicInfo
        = new WeaponInformation(
            "Magic",
            "10,000 Coins\nBehold. Magic Magic. It's like Magic but more magical. Also more damage and just all around better.");
    #endregion

    private void Awake()
    {
        informationPanel.Show(mainPanel.weaponName);
        //if (playerHud == null) playerHud = FindObjectOfType<PlayerHud>().gameObject;
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
        informationPanel.Show(mainPanel.weaponName);
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
            if(curHorizontal > 0)
            //Go Back
            {
                MovePanelsLeft();
            }
            else
            //Go Forward
            {
                MovePanelsRight();
            }
        }
    }

    public void MovePanelsLeft()
    {
        isMovingPanels = true;
        for (int i = 0; i < shopPanels.Length; i++)
        {
            ShopPanel nextPanel = shopPanels[(i + 1) % shopPanels.Length];
            nextPanel.StartMovingTo(shopPanels[i].rectTransform.localPosition, shopPanels[i].rectTransform.localScale, shopPanels[i].isMainPanel);
        }
    }

    public void MovePanelsRight()
    {
        isMovingPanels = true;
        for (int i = 0; i < shopPanels.Length; i++)
        {
            ShopPanel nextPanel = shopPanels[(i + 1) % shopPanels.Length];
            shopPanels[i].StartMovingTo(nextPanel.rectTransform.localPosition, nextPanel.rectTransform.localScale, nextPanel.isMainPanel);
        }
    }
    #endregion

    #region Button Calls
    public bool ChargePlayer(int amount)
    {
        if(amount <= Player.Instance.Coins)
        {
            Debug.Log($"Charged the Player {amount}.");
            Player.Instance.Coins -= amount;
            PlayerHud.Instance.SetCoinCount(Player.Instance.Coins);

            onSuccessfulSale();

            return true;
        }
        else
        {
            Debug.Log($"Player didn't have enough coins to pay {amount}");

            onFailedSale();

            return false;
        }
    }

    public void Upgrade(string weaponName, string upgradeName)
    {
        Debug.Log($"Upgraded {weaponName}.");
        GetWeaponInfo(weaponName).isUpgraded = true;
        Player.Instance.AddWeapon(WeaponManager.Instance.GetWeapon(upgradeName));
    }

    public void ActivateHubShop()
    {
        PlayerHud.Instance.gameObject.SetActive(false);

        Player.Instance.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        foreach (ShopPanel shopPanel in shopPanels)
        {
            if (Player.Instance.weapons.Find(w => w.name == shopPanel.weaponName))
            {
                GetWeaponInfo(shopPanel.weaponName).isUnlocked = true;
                if (shopPanel == mainPanel) shopPanel.BlackOut(false);
            }
        }

        gameObject.SetActive(true);
    }

    public void DeactivateHubShop()
    {
        PlayerHud.Instance.gameObject.SetActive(true);

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

        throw new ArgumentException($"No weapon with name {weaponName} exists");
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

    public WeaponInformation(string name, string description, bool isUnlocked = false)
    {
        this.name = name;
        this.description = description;

        this.isUnlocked = isUnlocked;
        isUpgraded = false;
    }
}
