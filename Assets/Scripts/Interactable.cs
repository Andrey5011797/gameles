using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string promptText = "Взаимодействовать";

    public UnityEvent onInteract;

    public void Interact()
    {
        onInteract?.Invoke();
        Debug.Log("Нажали Е на предмете: " + gameObject.name);
    }
}