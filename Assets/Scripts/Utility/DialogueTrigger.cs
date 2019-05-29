using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] List<string> text = new List<string>();
	[SerializeField] Sprite characterSpeaking = null;
	[SerializeField] string characterSpeakingName = "";
    private bool triggered;

	private void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.CompareTag("Player"))
        {
            if (triggered) { return; }

            triggered = true;
		    Player.Instance.enabled = false;
		    Cursor.lockState = CursorLockMode.None;
		    Cursor.visible = true;
            this.enabled = false;

            DialogueSystem.Instance.OnDialogueEnd += ReEnablePlayer;
            if(characterSpeaking != null) {
                DialogueSystem.Instance.SetCharacterImage(characterSpeaking);
            }
		    DialogueSystem.Instance.SetCharacterName(characterSpeakingName);

		    foreach (string item in text)
		    {
			    DialogueSystem.Instance.QueueDialogue(item, true);
		    }
        }
	}

	private void ReEnablePlayer()
	{

        DialogueSystem.Instance.OnDialogueEnd -= ReEnablePlayer;
        Player.Instance.enabled = true;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		this.enabled = false;
	}
}
