using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lavaMovement : MonoBehaviour
{
    // Start is called before the first frame update
    //scroll main texture based on time

    public Vector2 Scroll = new Vector2(0.05f, 0.05f);
    Vector2 Offset = new Vector2(0f, 0f);

    [SerializeField] Renderer renderer = null;

    void Update()
    {
        Offset += Scroll * Time.deltaTime;
        renderer.material.SetTextureOffset("_MainTex", Offset);
    }
}
