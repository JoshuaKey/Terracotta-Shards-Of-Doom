using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPowerUp : PowerUp {

    public Vector2Int CoinDropRange = new Vector2Int(7, 13);
    public int BaseValue = 1;
    public bool UseBigCoins = false;

    public override void GainPowerUp() {
        int amo = Random.Range(CoinDropRange.x, CoinDropRange.y);
        for (int i = 0; i < amo; i++) {
            Coin coin = UseBigCoins ? CoinPool.Instance.CreateBigCoin() : CoinPool.Instance.Create();
            Vector3 pos = this.transform.position + Random.insideUnitSphere * Random.value * 2.0f;
            pos += Vector3.up;
            coin.SetPosition(pos);
            coin.Value = BaseValue * LevelManager.Instance.GetWorld();
        }
    }
}
