using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] HubShop hubShopMenu;

    private Interactable interactable;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.Subscribe(OpenShop);
    }

    public void OpenShop()
    {
        hubShopMenu.ActivateHubShop();
    }
}
