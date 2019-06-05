using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextObject
{
	public string Text;
	public List<string> buttons = new List<string>();


	public void Reformat()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i] = InputController.Instance.GetActionText(buttons[i]);
		}

		string[] buttons2 = buttons.ToArray();

		Text = string.Format(Text, buttons2);
	}
}
