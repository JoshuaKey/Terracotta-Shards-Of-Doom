using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderLogic : MonoBehaviour
{
	LineRenderer lineyBoi;

	private void Start()
	{
		lineyBoi = this.GetComponent<LineRenderer>();
	}

	private void OnTriggerEnter(Collider other)
	{
		lineyBoi.enabled = true;
	}

	private void OnTriggerExit(Collider other)
	{
		lineyBoi.enabled = false;
	}
}
