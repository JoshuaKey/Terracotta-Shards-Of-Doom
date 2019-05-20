using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Vector3 openPos;
    public Vector3 closedPos;
    public float moveTime = 1.0f;
    public bool isOpen = false;

    [Header("Components")]
    [SerializeField] private GameObject wall;
    [SerializeField] private Collider wallCollider;

    void Start() 
    {
        if(wall == null) { wall = GetComponentInChildren<MeshRenderer>().gameObject; }
        if(wallCollider == null) { wallCollider = GetComponentInChildren<Collider>(); }

        if (isOpen) {
            wall.transform.localPosition = openPos;
            wallCollider.transform.localPosition = openPos;
        } else {
            wall.transform.localPosition = closedPos;
            wallCollider.transform.localPosition = closedPos;
        }
    }

    public void Open() 
    {
        if (!isOpen) {
            StartCoroutine(MoveWall(closedPos, openPos));
        }      
    }

    public void Close() 
    {
        if (isOpen) {
            StartCoroutine(MoveWall(openPos, closedPos));
        }      
    }

    private IEnumerator MoveWall(Vector3 startPos, Vector3 endPos) 
    {
        AudioManager.Instance.PlaySoundWithParent("door_opening", ESoundChannel.SFX, gameObject);
        wallCollider.transform.localPosition = endPos;

        float startTime = Time.time;
        while(Time.time < startTime + moveTime) {
            float t = (Time.time - startTime) / moveTime;

            Vector3 pos = Vector3.Lerp(startPos, endPos, t);

            wall.transform.localPosition = pos;

            yield return null;
        }

        wall.transform.localPosition = endPos;
    }
}
