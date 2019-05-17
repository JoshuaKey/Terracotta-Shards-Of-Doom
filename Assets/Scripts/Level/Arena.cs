using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public Collider arenaCollider;
    public List<Wall> walls = new List<Wall>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Game.Instance.PlayerTag)) 
        {
            arenaCollider.enabled = false;
            foreach(Wall w in walls) {
                w.Close();
            }
        }
    }

}
