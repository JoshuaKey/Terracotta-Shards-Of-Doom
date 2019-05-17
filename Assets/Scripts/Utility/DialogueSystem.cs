using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour {

    [Header("Components")]
    [SerializeField] Image characterImage;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI nameText;

    [Header("Values")]
    [SerializeField] float characterDisplayTime;

    private bool isFinished = true;
    private bool displaying = false;
    private bool skip = false;
    private List<string> dialogueQueue = new List<string>();
    private int currDialogue = 0;

    public delegate void Event();

    public event Event OnDialogueEnd;

    public static DialogueSystem Instance = null;

    private void Awake() {
        if (Instance != null) { Destroy(this.gameObject); return; }
        Instance = this;
    }

    private void Start() {
        if (!displaying) {
            this.gameObject.SetActive(false);
        }
    }

    public void QueueDialogue(string text, bool display = false) {
        dialogueQueue.Add(text);

        if (display) {
            StartDialogue();
        }
    }
    public void QueueDialogue(string[] text, bool display = false) {
        for (int i = 0; i < text.Length; i++) {
            dialogueQueue.Add(text[i]);
        }

        if (display) {
            StartDialogue();
        }
    }

    public void SetCharacterImage(Sprite image) {
        characterImage.sprite = image;
    }
    public void SetCharacterName(string name) {
        nameText.text = name;

        RectTransform nameRect = nameText.rectTransform;

        var size = nameText.GetPreferredValues(name, Mathf.Infinity, nameRect.rect.height);
        size.y = nameRect.sizeDelta.y;
        size.x += 80f;
        nameRect.sizeDelta = size;
    }

    private void StartDialogue() {
        this.gameObject.SetActive(true);

        if (!displaying) {
            isFinished = false;
            currDialogue = 0;
            StartCoroutine(DisplayDialogue(currDialogue));
        }
    }

    [ContextMenu("Continue Dialogue")]
    public void ContinueDialogue() {
        if (displaying) { // Skip
            skip = true;
        } else if (currDialogue + 1 >= dialogueQueue.Count) { // End Dialogue
            EndDialogue();
        } else {
            currDialogue++;
            StartCoroutine(DisplayDialogue(currDialogue)); // Continue
        }
    }
    public void BackDialogue() {
        if (displaying) { // Skip
            skip = true;
        } else if (currDialogue - 1 < 0) { // Nothing
            //EndDialogue();
        } else {
            currDialogue--;
            StartCoroutine(DisplayDialogue(currDialogue)); // Continue
        }
    }

    public void EndDialogue() {
        OnDialogueEnd?.Invoke();
        this.gameObject.SetActive(false);

        dialogueText.text = "";
        dialogueQueue.Clear();
        isFinished = true;
    }

    private IEnumerator DisplayDialogue(int index) {
        //if (dialogueQueue.Count <= 0) { yield break; }

        skip = false;
        displaying = true;
        dialogueText.text = "";

        //dialogueSource.Play(); // Audio

        float nextDisplayTime = 0f;
        string text = dialogueQueue[index];

        for (int y = 0; y < text.Length; y++) {
            while (!skip && nextDisplayTime > Time.time) {
                yield return null;
            }

            dialogueText.text += text[y];
            nextDisplayTime = Time.time + characterDisplayTime;
        }

        displaying = false;
        //dialogueSource.Stop();
    }

    public bool IsFinished() { return isFinished; }

}