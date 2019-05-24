using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] List<string> text = new List<string>();
	[SerializeField] Sprite characterSpeaking = null;
	[SerializeField] string characterSpeakingName = "";

	private void OnTriggerEnter(Collider other)
	{
		Player.Instance.enabled = false;
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		DialogueSystem.Instance.OnDialogueEnd += ReEnablePlayer;
		DialogueSystem.Instance.SetCharacterImage(characterSpeaking);
		DialogueSystem.Instance.SetCharacterName(characterSpeakingName);

		foreach (string item in text)
		{
			DialogueSystem.Instance.QueueDialogue(item, true);
		}
	}

	private void ReEnablePlayer()
	{
		Player.Instance.enabled = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		this.enabled = false;
	}
}
