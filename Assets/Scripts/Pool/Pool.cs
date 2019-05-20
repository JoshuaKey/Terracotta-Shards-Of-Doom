using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool<T> : PoolBase where T : PoolObject {

    [Header("Options")]
    [SerializeField] protected T Prefab = null;
    [SerializeField] protected int MaxAmo = 0;

    [SerializeField] protected List<T> ObjectPool;
    [SerializeField] protected int CurrAmo;

    void Start() {
        ObjectPool = new List<T>(MaxAmo);
        CurrAmo = 0;

        for (int i = 0; i < MaxAmo; i++) {
            T obj = Instantiate(Prefab, this.transform);
            obj.gameObject.SetActive(false);
            obj.name += "" + i;

            ObjectPool.Add(obj);
        }

        SceneManager.sceneLoaded += OnSceneChange;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode) {
        while(CurrAmo > 0) {
            T obj = ObjectPool[0];
            obj.gameObject.SetActive(false);
        }
    }

    public T Create() {
        if (CurrAmo >= MaxAmo) { return null; }

        T obj = ObjectPool[CurrAmo];
        obj.Create(this, CurrAmo++);
        obj.gameObject.SetActive(true);

        return obj;
    }
    public override void Destroy(int index) {
        if (index < 0 || index >= MaxAmo) {
            Debug.LogWarning(this.name + ": Index " + index + " is not valid!");
            return;
        }

        T obj = ObjectPool[index];
        if (obj.IsActive()) {
            Swap(index, --CurrAmo);
            ObjectPool[index].SetIndex(index);
            ObjectPool[CurrAmo].SetIndex(CurrAmo);
        }
        obj.Reset();
        obj.gameObject.SetActive(false);
    }

    private void Swap(int a, int b) {
        T temp = ObjectPool[a];
        ObjectPool[a] = ObjectPool[b];
        ObjectPool[b] = temp;
    }

    public int GetMaxAmo() {
        return MaxAmo;
    }
    public int GetCurrAmo() {
        return CurrAmo;
    }
    public T GetPrefab() {
        return Prefab;
    }
}
