using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    public bool isMainPanel;
    public string weaponName;

    [HideInInspector] public RectTransform rectTransform;

    HubShop hubShop;
    Image image;
    RawImage rawImage;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        hubShop = GetComponentInParent<HubShop>();
        image = GetComponentsInChildren<Image>()[1];
        rawImage = GetComponentInChildren<RawImage>();

        BlackOut(!HubShop.GetWeaponInfo(weaponName).isUnlocked);
    }

    public void StartMovingTo(Vector3 pos, Vector3 scale, bool isMainPanel)
    {
        StartCoroutine(MoveTo(pos, scale, 0.25f, isMainPanel));
    }

    IEnumerator MoveTo(Vector3 pos, Vector3 scale, float duration, bool isMainPanel)
    {
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, pos, (Time.time - startTime));
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, scale, (Time.time - startTime));
            yield return null;
        }

        rectTransform.localPosition = pos;
        rectTransform.localScale = scale;
        this.isMainPanel = isMainPanel;

        if (isMainPanel) hubShop.mainPanel = this;

        BlackOut(!HubShop.GetWeaponInfo(weaponName).isUnlocked);
        hubShop.isMovingPanels = false;
    }

    public void BlackOut(bool isBlackedOut)
    {
        if(image != null) image.color = isBlackedOut ? Color.black : Color.white;
        if (rawImage != null) rawImage.color = isBlackedOut ? Color.black : Color.white;
    }
}
