using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour {

    protected PoolBase pool = null;
    protected int index = -1;
    protected bool isActive = false;

    protected virtual void Start() { }

    public void Create(PoolBase _pool, int _index) {
        pool = _pool;
        index = _index;
        isActive = true;
        Start();
    }
    public void Destroy() {
        if (pool != null) {
            pool.Destroy(index);
        }
        this.gameObject.SetActive(false);
        Reset();
    }
    public virtual void Reset() {
        pool = null;
        index = -1;
        isActive = false;
    }

    protected virtual void OnDisable() {
        if (IsActive()) {
            Destroy();
        }       
    }

    public void SetIndex(int i) { index = i; }

    public bool IsActive() { return isActive; }

}
