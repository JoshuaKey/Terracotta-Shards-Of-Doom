using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaDoor : MonoBehaviour
{
	[SerializeField] float lowerPosition;
	[SerializeField] float upperPosition;

	private bool closing = false;
	private float lerpChange = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (closing)
		{
			float currentY = this.transform.position.y;
			//lerpChange = Mathf.Clamp01(lerpChange + Time.deltaTime);
			float newY = Mathf.Lerp(currentY, upperPosition, Time.deltaTime);
			this.transform.position = new Vector3(this.transform.position.x, newY, this.transform.position.z);
			if(newY == upperPosition)
			{
				this.enabled = false;
			}
		}
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			closing = true;
		}
	}
}
