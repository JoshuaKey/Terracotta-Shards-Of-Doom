using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : Pool<Coin> {

    public static CoinPool Instance;

    private void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

    public Coin CreateBigCoin() {
        Coin coin = Create();

        coin.transform.localScale = Vector3.one * 2;

        return coin;
    }

}
