using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public Collider arenaCollider;
    public List<Wall> walls = new List<Wall>();
    public bool playerEnteredArena = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Game.Instance.PlayerTag)) 
        {
            playerEnteredArena = true;
            arenaCollider.enabled = false;
            foreach(Wall w in walls) {
                w.Close();
            }
        }
    }

}
