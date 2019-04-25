using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    
    private bool visited;

    public bool Visited
    {
        get { return visited; }
        set { visited = value; }
    }
    
    [SerializeField]
    Vector3 offset = Vector3.zero;

    [SerializeField]
    Vector3 halfExtents = Vector3.zero;

    [HideInInspector]
    public List<Pot> pots = null;

    void Start()
    {
        pots = new List<Pot>();
        Collider[] colliders = Physics.OverlapBox(transform.position + offset, halfExtents);

        Pot p;
        foreach(Collider c in colliders)
        {
            if(p = c.GetComponent<Pot>())
            {
                pots.Add(p);
            }
        }
    }

    void Update()
    {
    }
    
}
