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

    private void Start()
    {
        target.transform.parent = null;
    }

    public void StartFall()
    {
        if (!falling)
        {
            gameObject.SetActive(true);
            StartCoroutine(Fall());
        }
    }

    //Used to prevent clipping with the mesh below it
    readonly Vector3 TargetOffset = new Vector3(0.0f, .2f, 0.0f);
    IEnumerator Fall()
    {
        falling = true;
        target.gameObject.SetActive(true);

        target.transform.position = targetPosition + TargetOffset;

        while ((transform.position - targetPosition).magnitude > .1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 5.0f);
            yield return null;
        }

        falling = false;
        landed = true;
        gameObject.SetActive(false);
        target.gameObject.SetActive(false);
    }
}
