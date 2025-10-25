using UnityEngine;

public class ToggleAnimatedObject : MonoBehaviour
{
    public GameObject targetObject;  // The animated GameObject

    void OnMouseDown()
    {
        if (targetObject != null)
            targetObject.SetActive(!targetObject.activeSelf);
    }
}
