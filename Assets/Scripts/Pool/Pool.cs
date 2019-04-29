using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool<T> : PoolBase where T : PoolObject {

    //[Header("Options")]
    //[SerializeField] private T Prefab;
    //[SerializeField] private int MaxAmo;

    //private List<T> ObjectPool;
    //private int CurrAmo;

    //// Start is called before the first frame update
    //void Start() {
    //    ObjectPool = new List<T>(MaxAmo);
    //    CurrAmo = 0;

    //    for (int i = 0; i < MaxAmo; i++) {
    //        T obj = Instantiate(Prefab, this.transform);
    //        obj.gameObject.SetActive(false);          

    //        ObjectPool.Add(obj);
    //    }

    //    SceneManager.sceneLoaded += OnSceneChange;
    //}

    //private void OnSceneChange(Scene scene, LoadSceneMode mode) {
    //    CurrAmo = 0;
    //    ObjectPool.ForEach(o => {  o.Destroy();  });
    //}

    //public T Create() {
    //    if(CurrAmo >= MaxAmo) { return null; }

    //    T obj = ObjectPool[CurrAmo];
    //    obj.Create(this, CurrAmo++);
    //    obj.gameObject.SetActive(true);

    //    return obj;
    //}
    //public override void Destroy(int index) {
    //    if(index < 0 || index >= MaxAmo) {
    //        Debug.LogWarning(this.name  + ": Index " + index + " is not valid!");
    //        return;
    //    }

    //    T obj = ObjectPool[CurrAmo];
    //    Swap(index, CurrAmo--);

    //    obj.transform.SetParent(this.transform, false);
    //}

    //private void Swap(int a, int b) {
    //    T temp = ObjectPool[a];
    //    ObjectPool[a] = ObjectPool[b];
    //    ObjectPool[b] = temp;
    //}

    //public int GetMaxAmo() {
    //    return MaxAmo;
    //}
    //public int GetCurrAmo() {
    //    return CurrAmo;
    //}
    //public T GetPrefab() {
    //    return Prefab;
    //}
}
