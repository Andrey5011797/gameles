using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class WakeUpSequence : MonoBehaviour
{
    [Header("Ссылки")]
    public CanvasGroup blackScreen;
    public GameObject promptPanel;
    public TMPro.TextMeshProUGUI promptText;
    public PlayerMovement playerMovement;

    private bool _isWokenUp = false;
    private bool _isNearBed = false;

    void Start()
    {
        blackScreen.alpha = 1f;
        blackScreen.blocksRaycasts = true;

        if (playerMovement != null) playerMovement.enabled = false;
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    void Update()
    {
        if (_isWokenUp) return;
        if (_isNearBed)
        {
            if (!promptPanel.activeSelf) promptPanel.SetActive(true);
            promptText.text = "[E] Проснуться";

            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                StartCoroutine(FadeOutRoutine());
            }
        }
        else
        {
            if (promptPanel.activeSelf) promptPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isNearBed = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isNearBed = false;
        }
    }

    IEnumerator FadeOutRoutine()
    {
        _isWokenUp = true;
        if (promptPanel != null) promptPanel.SetActive(false);

        float fadeSpeed = 1.5f;
        float timer = 0f;

        while (timer < fadeSpeed)
        {
            timer += Time.deltaTime;
            blackScreen.alpha = Mathf.Lerp(1f, 0f, timer / fadeSpeed);
            yield return null;
        }

        blackScreen.alpha = 0f;
        blackScreen.blocksRaycasts = false;

        if (playerMovement != null) playerMovement.enabled = true;
    }
}