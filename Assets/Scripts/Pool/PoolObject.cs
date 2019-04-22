using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    private PoolBase pool = null;
    private int index = -1;
    private bool isActive = false;

    public void Create(PoolBase _pool, int _index) {
        pool = _pool;
        index = _index;
        isActive = true;
    }
    public void Destroy() {
        pool.Destroy(index);
        Reset();
    }
    private void Reset() {
        pool = null;
        index = -1;
        isActive = false;
    }

    private void OnDisable() {
        if (isActive) {
            Destroy();
        }        
    }

}
