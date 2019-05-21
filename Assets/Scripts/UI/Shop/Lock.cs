using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public void AfterUnlock()
    {
        Destroy(gameObject);
    }
}
