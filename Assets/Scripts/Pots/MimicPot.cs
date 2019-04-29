using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicPot : MonoBehaviour
{

    [SerializeField] public float aggroRadius = 5f; // this is when the pot first wakes up
    [SerializeField] public float chaseRadius = 10.0f; // this is after the pot is awake
    //This is all atack stuff and i currently dont wanna change it
    [SerializeField] public float attackRadius = 2.5f;
    [SerializeField] public float attackDuration = 0.25f;
    [SerializeField] public float attackAngle = 70f;

    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool hasHitPlayer = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
