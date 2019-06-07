using Luminosity.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TextScroll : MonoBehaviour
{
    [SerializeField] float normalSpeed = 1;
    [SerializeField] float buttonSpeed = 2;
    [SerializeField] float endPos;
    [SerializeField] UnityEvent onEnd;

    Vector3 origin;
    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        origin = rect.position;

        Debug.Log(Time.timeScale);
    }

    private void Update()
    {
        float speed = InputManager.GetButton("UI_Submit") ? buttonSpeed : normalSpeed;

        Vector3 newPos = rect.position;
        newPos += (Vector3.up * speed * Time.deltaTime);

        rect.position = newPos;

        if(rect.localPosition.y >= endPos)
        {
            onEnd.Invoke();
        }
    }

    private void OnDisable()
    {
        if(rect != null) rect.position = origin;
    }
}
