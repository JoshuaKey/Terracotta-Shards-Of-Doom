using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASD : MonoBehaviour
{
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = Vector3.zero;

        velocity.x = Input.GetAxis("Horizontal");
        velocity.z = Input.GetAxis("Vertical");

        velocity *= speed * Time.deltaTime;

        //rotate so S always goes towards camera and W away
        Quaternion rotateBy = Camera.main.transform.rotation;
        velocity = Quaternion.AngleAxis(rotateBy.eulerAngles.y, Vector3.up) * velocity;

        transform.position += velocity;
    }
}
