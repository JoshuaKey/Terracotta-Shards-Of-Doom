using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPool : Pool<Coin> {

    public Coin CreateBigCoin() {
        Coin coin = Create();

        coin.coinModel.localScale = Vector3.one;

        return coin;
    }
}
