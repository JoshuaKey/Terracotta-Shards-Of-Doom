using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    protected PoolBase pool = null;
    protected int index = -1;
    protected bool isActive = false;

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
