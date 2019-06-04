using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] List<TextObject> text = new List<TextObject>();
	[SerializeField] Sprite characterSpeaking = null;
	[SerializeField] string characterSpeakingName = "";
	[SerializeField] bool IsTutorial = false;
    private bool triggered;

    public static List<string> DialoguesHit = new List<string>();

    private void Awake() {
        if (DialoguesHit.Contains(this.name)) {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
	{
        if (triggered) { return; }

        if (DialoguesHit.Contains(this.name)) {
            Destroy(this.gameObject);
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            triggered = true;
            if (this.IsTutorial) {
                DialoguesHit.Add(this.name);
            }

            if (!this.IsTutorial || (!Player.Instance.SkipTutorial && this.IsTutorial))
			{
				Player.Instance.enabled = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				this.enabled = false;

				DialogueSystem.Instance.OnDialogueEnd += ReEnablePlayer;
				if(characterSpeaking != null) {
				    DialogueSystem.Instance.SetCharacterImage(characterSpeaking);
				}
				DialogueSystem.Instance.SetCharacterName(characterSpeakingName);

				foreach (TextObject item in text)
				{
					item.Reformat();
				    DialogueSystem.Instance.QueueDialogue(item.Text, true);
				}
			}
        }
	}

    private void OnDestroy() {
        DialogueSystem.Instance.OnDialogueEnd -= ReEnablePlayer;
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
