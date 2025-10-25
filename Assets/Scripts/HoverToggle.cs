using UnityEngine;

public class HoverToggleChild : MonoBehaviour
{
    public GameObject childSprite;

    void OnMouseEnter()
    {
        if (childSprite != null)
            childSprite.SetActive(true);
    }

    void OnMouseExit()
    {
        if (childSprite != null)
            childSprite.SetActive(false);
    }
}
