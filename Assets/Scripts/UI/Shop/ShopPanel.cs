using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public bool isMainPanel;
    public string weaponName;

    #pragma warning disable 0649
    [SerializeField] Animator lockModel;
    #pragma warning restore 0649

    [HideInInspector] public RectTransform rectTransform;

    HubShop hubShop;
    RawImage rawImage;

    private Vector3? desiredPos = null;
    private Vector3? desiredScale = null;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        hubShop = GetComponentInParent<HubShop>();
        rawImage = GetComponentInChildren<RawImage>();

        BlackOut(!HubShop.GetWeaponInfo(weaponName).isUnlocked);
    }

    public void StartMovingTo(Vector3 pos, Vector3 scale, bool isMainPanel)
    {
        StartCoroutine(MoveTo(pos, scale, 0.25f, isMainPanel));
    }

    IEnumerator MoveTo(Vector3 pos, Vector3 scale, float duration, bool isMainPanel)
    {
        desiredPos = pos;
        desiredScale = scale;

        float startTime = Time.time;
        this.isMainPanel = isMainPanel;

        while (Time.time < startTime + duration)
        {
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, pos, (Time.time - startTime));
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, scale, (Time.time - startTime));
            yield return null;
        }

        rectTransform.localPosition = pos;
        rectTransform.localScale = scale;

        if (isMainPanel)
        {
            hubShop.mainPanel = this;
            BlackOut(!HubShop.GetWeaponInfo(weaponName).isUnlocked);
        }

        hubShop.isMovingPanels = false;
    }

    public void BlackOut(bool isBlackedOut)
    {
        if (!isBlackedOut && lockModel != null) lockModel.SetTrigger("Unlock");
    }

    public void MakeMainPanel()
    {
        Debug.Log($"{weaponName} clicked");
        StartCoroutine(MoveUntilMainPanel());
    }

    private IEnumerator MoveUntilMainPanel()
    {
        while(!isMainPanel)
        {
            if(!hubShop.isMovingPanels)
            {
                hubShop.MovePanelsLeft();
            }
            yield return null;
        }
    }

    private void OnDisable()
    {
        if(desiredPos != null) rectTransform.localPosition = (Vector3)desiredPos;
        if(desiredPos != null) rectTransform.localScale = (Vector3)desiredScale;
        hubShop.isMovingPanels = false;
    }
}
