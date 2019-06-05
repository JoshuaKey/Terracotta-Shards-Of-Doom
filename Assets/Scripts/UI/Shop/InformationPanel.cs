using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InformationPanel : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] TextMeshProUGUI weaponName;
    [SerializeField] TextMeshProUGUI weaponDescription;
    [SerializeField] Button button;
    #pragma warning restore 0649

    HubShop hubShop;

    //string[] descriptions =
    //{
    //    //Sword Description
    //    "The old faithful of many an adventurer. It slices and dices and not much else. But what if it was on fire?",
    //    //Bow Description
    //    "The problem with ranged weapons is it takes time to load. This upgrade keeps your enemies frosty so you can take your time.",
    //    //Hammer Description
    //    "Keep it simple taken to a stupid degree. Why use a hammer when a rock on a stick deals more damage?",
    //    //Spear Description
    //    "If only your spear could stab more than one pot. This upgrade is the closest we could get. Any bystanders to your rampage will be in for a nasty shock.",
    //    //Crossbow Description
    //    "We started from the ground up with this one. We call it a magic missile. Still does the same thing though, just Bigger.",
    //    //Magic Description
    //    "Behold. Magic Magic. It's like Magic but more magical. Also more damage and just all around better.",
    //};

    private void Start()
    {
        hubShop = GetComponentInParent<HubShop>();
    }

    public void Show(string weaponName)
    {
        gameObject.SetActive(true);

        if (HubShop.GetWeaponInfo(weaponName).isUnlocked)
        {
            if(HubShop.GetWeaponInfo(weaponName).isUpgraded)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }

            this.weaponName.SetText(weaponName);

            weaponDescription.SetText(HubShop.GetWeaponInfo(weaponName).description);
        }
        else
        {
            this.weaponName.SetText("Locked");

            weaponDescription.SetText("That silhouette looks cool. Too bad you haven't unlocked that weapon yet. Go and find it then come back.");

            button.interactable = false;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void PurchaseUpgrade()
    {
        if (!HubShop.GetWeaponInfo(weaponName.text).isUpgraded)
        {
            switch (weaponName.text)
            {
                case "Sword": SwordButtonClick(); break;
                case "Bow": BowButtonClick(); break;
                case "Hammer": HammerButtonClick(); break;
                case "Spear": SpearButtonClick(); break;
                case "Crossbow": CrossbowButtonClick(); break;
                case "Magic": MagicButtonClick(); break;
            }
        }
    }

    public void SwordButtonClick()
    {
        if(hubShop.ChargePlayer(2000))
        {
            hubShop.Upgrade("Sword", "Fire Sword");
        }
    }

    public void BowButtonClick()
    {

        if (hubShop.ChargePlayer(4000))
        {
            hubShop.Upgrade("Bow", "Ice Bow");
        }
    }

    public void HammerButtonClick()
    {
        if(hubShop.ChargePlayer(6000))
        {
            hubShop.Upgrade("Hammer", "Rock Hammer");
        }
    }

    public void SpearButtonClick()
    {
        if(hubShop.ChargePlayer(8000))
        {
            hubShop.Upgrade("Spear", "Lightning Spear");
        }
    }

    public void CrossbowButtonClick()
    {
        if(hubShop.ChargePlayer(12500))
        {
            hubShop.Upgrade("Crossbow", "Magic Missile");
        }
    }

    public void MagicButtonClick()
    {
        if(hubShop.ChargePlayer(10000))
        {
            hubShop.Upgrade("Magic", "Magic Magic");
        }
    }
}
