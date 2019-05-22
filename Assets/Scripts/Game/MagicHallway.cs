using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHallway : MonoBehaviour
{
    [SerializeField]
    GameObject NextLocation = null;

    private Player player = null;

    private void Start()
    {
        player = Player.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject == Player.Instance)
        if (other.CompareTag(Game.Instance.PlayerTag))
        {
            Debug.Log("fuck me a little " + gameObject.name);
            Debug.Log(Player.Instance.transform.position);
            Debug.Log(this.transform.position);
            Debug.Log(NextLocation.transform.position);

            player.transform.position = NextLocation.transform.position;
            player.velocity = Vector3.zero;
            Player.Instance.CanMove = false;
            //player.LookTowards(NextLocation.transform.forward);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Game.Instance.PlayerTag))
        {
            Player.Instance.CanMove = true;
        }
    }
}
