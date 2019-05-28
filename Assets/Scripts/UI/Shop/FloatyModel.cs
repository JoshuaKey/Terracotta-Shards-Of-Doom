using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatyModel : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] Vector3 axis;
    [SerializeField] float rotateSpeed;
    [SerializeField] float speed;
    [SerializeField] float magnitude;
    #pragma warning restore 0649

    Vector3 origin;

    private void Start()
    {
        origin = transform.position;
    }

    private void Update()
    {
        transform.Rotate(axis, rotateSpeed * Time.deltaTime);

        Vector3 up = Vector3.up * Mathf.Sin(Time.time * speed) * magnitude;
        transform.position = origin + up;
    }
}
