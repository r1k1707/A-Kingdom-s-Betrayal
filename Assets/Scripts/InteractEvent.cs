using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractEvent : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI golemDialogue;
    public GameObject nextButton;
    public GameObject interactButton;

    public void Start()
    {
        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(false);
        }
    }
    public void OnClick()
    {
        if (dialogueBox != null)
        {
            dialogueBox.SetActive(true);
        }
        if (interactButton != null)
        {
            interactButton.SetActive(false);
        }
        if (nextButton != null)
        {
            nextButton.SetActive(true);
        }
    }

}
