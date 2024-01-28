using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    Queue<string> sentences;
    [SerializeField] Text nameText;
    [SerializeField] Text dialogueText;
    [SerializeField] Animator nameAnimator;
    [SerializeField] Animator dialogueAnimator;
    Coroutine fillTextBox;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(string characterName, string[] characterSentences, string state)
    {
        nameAnimator.SetBool("isOpen", true);
        dialogueAnimator.SetBool("isOpen", true);

        nameText.text = characterName;
        sentences.Clear();

        foreach (string sentence in characterSentences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence(state);
    }
    public string DisplayNextSentence(string state)
    {
        if (sentences.Count == 0)
        {
            state = "default";
            EndDialog(state);
            return state;
        }
        string sentence = sentences.Dequeue();
       try
        {
            StopCoroutine(fillTextBox);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        fillTextBox = StartCoroutine(TypeSentence(sentence));
        return null;
    }

    public string EndDialog(string state)
    {
        nameAnimator.SetBool("isOpen", false);
        dialogueAnimator.SetBool("isOpen", false);
        Debug.Log("No more lines");
        //state = "default";
        return state;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }
}
