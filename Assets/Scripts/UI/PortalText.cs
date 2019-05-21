using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalText : MonoBehaviour
{
	[SerializeField] Text portalName = null;
	[SerializeField] Player player = null;
	[SerializeField] Canvas canvas = null;
	[SerializeField] string text = "";

	// Use this for initialization
	void Start()
	{
		player = Player.Instance;	
		portalName.text = text;
	}

	// Update is called once per frame
	void Update()
	{
		if(portalName.text != text)
		{
			portalName.text = text;
		}

		canvas.transform.rotation = Quaternion.identity;
		canvas.transform.LookAt(player.transform.position);
	}
}
