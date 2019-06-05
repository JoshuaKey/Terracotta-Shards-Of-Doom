using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour
{
    [SerializeField] HubShop hubShopMenu;

    private Interactable interactable;
    private Animator animator;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        interactable.Subscribe(OpenShop);

        animator = GetComponentInChildren<Animator>();

        hubShopMenu.onSuccessfulSale += PlaySuccessAnimation;
        hubShopMenu.onFailedSale += PlayFailureAnimation;

        hubShopMenu.gameObject.SetActive(false);
    }

    public void PlaySuccessAnimation()
    {
        animator.SetTrigger("Success");
    }

    public void PlayFailureAnimation()
    {
        animator.SetTrigger("Failure");
    }

    private void Update()
    {
        transform.LookAt(Player.Instance.transform, Vector3.up);

        Vector3 eulers = transform.rotation.eulerAngles;
        eulers.x = 0;

        transform.rotation = Quaternion.Euler(eulers);
    }

    public void OpenShop()
    {
        hubShopMenu.ActivateHubShop();
    }
}
