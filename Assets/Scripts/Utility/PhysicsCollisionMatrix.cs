using UnityEngine;
using System.Collections.Generic;

public class PhysicsCollisionMatrix : MonoBehaviour {
    private Dictionary<int, int> _masksByLayer;

    public static PhysicsCollisionMatrix Instance;

    public void Start() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;

        _masksByLayer = new Dictionary<int, int>();
        for (int i = 0; i < 32; i++) {
            int mask = 0;
            for (int j = 0; j < 32; j++) {
                if (!Physics.GetIgnoreLayerCollision(i, j)) {
                    mask |= 1 << j;
                }
            }
            _masksByLayer.Add(i, mask);
        }
    }

    public int MaskForLayer(int layer) {
        return _masksByLayer[layer];
    }
}