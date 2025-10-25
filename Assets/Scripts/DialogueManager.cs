using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] int dialogueIndex;
    [SerializeField] TextMeshProUGUI dialogueText;
    [TextArea]
    [SerializeField] List<string> dialogue;


    private void Start()
    {
        dialogueText.text = dialogue[dialogueIndex];
    }

    public void NextDialogue()
    {
        dialogueIndex += 1;
        if (dialogueIndex >= dialogue.Count)
        {
            Debug.Log("wahoo!");
            StartCoroutine(DelaySceneLoad());
        }    
        else
        {
            dialogueText.text = dialogue[dialogueIndex];
        }
    }
    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadSceneAsync(sceneName);
    }
}