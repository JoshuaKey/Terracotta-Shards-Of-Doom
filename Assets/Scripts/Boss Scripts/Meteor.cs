using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField]
    GameObject target = null;

    private bool landed = false;

    public bool Landed
    {
        get { return landed; }
    }

    private Vector3 targetPosition;

    public Vector3 TargetPosition
    {
        get { return targetPosition; }
        set
        {
            landed = false;
            targetPosition = value;
        }
    }

    private bool falling = false;

    public void StartFall()
    {
        if(!falling)
        {
            StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        falling = true;
        target.SetActive(true);
        target.transform.position = targetPosition;

        while ((transform.position - targetPosition).magnitude > .1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            yield return null;
        }

        falling = false;
        target.SetActive(false);
        landed = true;
    }
}
