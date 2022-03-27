using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;
    public Text Conversation;
    public Text nameText;

    public Animator textBoxAnimator;

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogues dialogues)
    {
        textBoxAnimator.SetBool("isOpen",true);

        sentences.Clear();
        nameText.text = dialogues.charName;
        foreach (string sen in dialogues.sentence)
        {
            sentences.Enqueue(sen);
        }
        DisplayNextSentences();
    }

    public void DisplayNextSentences()
    {
        if (sentences.Count == 0)
        {
            DialogEnd();
            return;
        }
        string Sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine( TypeSentence(Sentence));
    }
    IEnumerator TypeSentence(string s)
    {
        Conversation.text = "";
        foreach (char letter in s.ToCharArray())
        {
            Conversation.text += letter;
            yield return null;
        }
        
    }
    public void DialogEnd()
    {
        textBoxAnimator.SetBool("isOpen",false);
        Debug.Log("The end");
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Z))
        {
            StopAllCoroutines();
        }    
    }
}
