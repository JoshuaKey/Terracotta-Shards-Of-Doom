using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.AddComponent<Rigidbody>();
        print("start");
        print(this.transform.position);
        this.transform.position = Vector3.zero;
        print(this.transform.position);

        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        Update();
        //Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        print(Time.deltaTime);
        this.transform.position = this.transform.position + Vector3.forward * Time.deltaTime * 10;
        print(this.transform.position);
        this.transform.Rotate(Vector3.forward * Time.deltaTime * 900);
        print("here");
        rb.AddForce(Vector3.forward);
    }
}
