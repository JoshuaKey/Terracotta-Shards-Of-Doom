using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScroll : MonoBehaviour
{
    [SerializeField] float speed = 1;

    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();

        Debug.Log(Time.timeScale);
    }

    private void Update()
    {
        Vector3 newPos = rect.position;
        newPos += (Vector3.up * speed * Time.deltaTime);

        rect.position = newPos;
    }
}
